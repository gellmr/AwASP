import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root' // Singleton service available globally across the entire application
})
export class CartService
{
  private http             = inject(HttpClient);
  
  private cartLines: any[] = [];                              // Private array holding the internal state of cart items.
  private cartSubject      = new BehaviorSubject<any[]>([]);  // BehaviorSubject holds the current cart data and broadcasts updates to any subscribers.
  public  cart$            = this.cartSubject.asObservable(); // Public observable stream exposed to components so they can listen to cart changes.
  
  private orders: any[]    = [];
  private ordersSubject    = new BehaviorSubject<any[]>([]);
  public  orders$          = this.ordersSubject.asObservable();

  // The guest and user objects are stored in browser local storage using stringified json
  public guest: any = this.loadGuestFromStorage();
  public user: any  = this.loadUserFromStorage();

  private loadGuestFromStorage(): any {
    try {
      const stored = localStorage.getItem('guest'); return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  private loadUserFromStorage(): any {
    try {
      const stored = localStorage.getItem('user'); return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  // Update user state, handle local storage persistence, and transition session data
  setUser(userData: any)
  {
    this.user = userData;
    if (userData === null)
    {
      // We are transitioning to logged out (eg guest, or completely anonymous state)
      // Clear the user json object from local storage.
      // Local myorders will become blank.
      // Activate a new observer to track myorders for guest.
      try {
        localStorage.removeItem('user');
      }
      catch {}
      this.fetchMyOrders().subscribe(); // This forces a POST to /api/myorders/fetch-orders
    }
    else
    {
      // We are not transitioning to logged out.
      // There is a current user object.
      // Update the local storage for the user object.
      // Clear any guest object.
      try {
        localStorage.setItem('user', JSON.stringify(userData));
        localStorage.removeItem('guest');
      }
      catch {}
      this.guest = null;
    }
  }

  // Bootstrap the service state on app load sequentially and reactively
  init(): Observable<any> {
    const guest$ = this.guest ? of(this.guest) : this.fetchGuest();
    return guest$.pipe(
      switchMap(() => this.fetchCart())
    );
  }

  // Helper to extract guest ID safely if guest object exists
  getGuestId(): string | null {
    return this.guest ? this.guest.id : null;
  }

  // Return current raw cart lines array
  getCartLines(): any[] {
    return this.cartLines;
  }

  // Return current raw orders array
  getOrders(): any[] {
    return this.orders;
  }

  // Fetch guest session from server and cache it locally
  fetchGuest(): Observable<any> {
    return this.http.get<any>('/api/guest').pipe(
      tap({
        next: (guest) => {
          this.guest = guest;
          try {
            localStorage.setItem('guest', JSON.stringify(guest));
          }
          catch {}
        },
        error: (err) => {
          console.error('Error fetching guest session:', err);
        }
      })
    );
  }

  // Fetch current cart items from server and push update into the stream
  fetchCart(): Observable<any[]> {
    return this.http.get<any[]>('/api/cart').pipe(
      tap({
        next: (cartLines) => {
          this.cartLines = cartLines || [];
          this.cartSubject.next([...this.cartLines]);
        },
        error: (err) => {
          console.error('Error fetching cart lines:', err);
        }
      })
    );
  }

  // Update quantity of an item with optimistic UI updates (instant feedback before server responds)
  updateCartQuantity(cartLineID: number | null, qty: number, isp: any) {
    const payload = { cartLineID, qty, isp };

    // Optimistically update the local state first to make it instantaneous in the UI
    const existingIndex = this.cartLines.findIndex(line => 
      (cartLineID !== null && line.cartLineID === cartLineID) || (line.isp.id === isp.id)
    );

    if (existingIndex === -1) {
      if (qty > 0) {
        this.cartLines.push({ cartLineID, qty, isp });
      }
    } else {
      if (qty === 0) {
        this.cartLines.splice(existingIndex, 1);
      } else {
        this.cartLines[existingIndex].qty = qty;
      }
    }
    // Broadcast the optimistic state change immediately
    this.cartSubject.next([...this.cartLines]);

    // Send update request to server in the background
    this.http.post<any>('/api/cart/update', payload).subscribe({
      next: (serverCartLine) => {
        const index = this.cartLines.findIndex(line => line.isp.id === isp.id);
        const serverIsp = serverCartLine.isp ? JSON.parse(JSON.stringify(serverCartLine.isp)) : null;
        const isRemoval = serverCartLine.qty === 0;

        if (index !== -1) {
          // If local quantity matches the server confirmation, merge cleanly
          if (this.cartLines[index].qty === qty) {
            if (isRemoval) {
              this.cartLines.splice(index, 1);
            } else {
              this.cartLines[index] = {
                cartLineID: serverCartLine.cartLineID,
                qty: serverCartLine.qty,
                isp: serverIsp
              };
            }
            this.cartSubject.next([...this.cartLines]);
          } else {
            // A newer user action occurred while waiting; preserve new quantity but update ID
            this.cartLines[index].cartLineID = serverCartLine.cartLineID;
          }
        }
      },
      error: (err) => {
        console.error('Error updating cart on server:', err);
        // Fallback: Re-sync with server state if request fails
        this.fetchCart();
      }
    });
  }

  // Clear local cart state instantly
  clearCart() {
    this.cartLines = [];
    this.cartSubject.next([...this.cartLines]);
  }

  // Clear cart locally and sync clearing action with the server
  clearCartOnServer() {
    this.clearCart();
    this.http.post<any>('/api/cart/clear', {}).subscribe({
      next: () => {
        this.fetchCart();
      },
      error: (err) => {
        console.error('Error clearing cart on server:', err);
        this.fetchCart();
      }
    });
  }

  // Fetch user or guest order history and broadcast via orders$ stream
  fetchMyOrders(): Observable<any> {
    const uid = this.user ? this.user.appUserId : null;
    const gid = this.getGuestId();

    if (!uid && !gid) {
      this.orders = [];
      this.ordersSubject.next([...this.orders]);
      return new Observable(subscriber => {
        subscriber.next({ rows: [] });
        subscriber.complete();
      });
    }

    const payload = { uid, gid };
    return this.http.post<any>('/api/myorders/fetch-orders', payload).pipe(
      tap({
        next: (res) => {
          this.orders = res.rows || [];
          this.ordersSubject.next([...this.orders]);
        },
        error: (err) => {
          console.error('Error fetching orders:', err);
          this.orders = [];
          this.ordersSubject.next([...this.orders]);
        }
      })
    );
  }
}
