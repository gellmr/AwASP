import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private http = inject(HttpClient);

  private cartLines: any[] = [];
  private cartSubject = new BehaviorSubject<any[]>([]);
  public cart$ = this.cartSubject.asObservable();

  private orders: any[] = [];
  private ordersSubject = new BehaviorSubject<any[]>([]);
  public orders$ = this.ordersSubject.asObservable();

  public guest: any = this.loadGuestFromStorage();
  public user: any = this.loadUserFromStorage();

  private loadGuestFromStorage(): any {
    try {
      const stored = localStorage.getItem('guest');
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  private loadUserFromStorage(): any {
    try {
      const stored = localStorage.getItem('user');
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  setUser(userData: any) {
    this.user = userData;
    if (userData === null) {
      try {
        localStorage.removeItem('user');
      } catch {}
      this.fetchMyOrders().subscribe();
    } else {
      try {
        localStorage.setItem('user', JSON.stringify(userData));
        localStorage.removeItem('guest');
      } catch {}
      this.guest = null;
    }
  }

  init() {
    if (!this.guest) {
      this.fetchGuest().subscribe();
    } else {
      this.fetchCart();
    }
  }

  getGuestId(): string | null {
    return this.guest ? this.guest.id : null;
  }

  getCartLines(): any[] {
    return this.cartLines;
  }

  getOrders(): any[] {
    return this.orders;
  }

  fetchGuest(): Observable<any> {
    return this.http.get<any>('/api/guest').pipe(
      tap({
        next: (guest) => {
          this.guest = guest;
          try {
            localStorage.setItem('guest', JSON.stringify(guest));
          } catch {}
          this.fetchCart();
        },
        error: (err) => {
          console.error('Error fetching guest session:', err);
        }
      })
    );
  }

  fetchCart() {
    this.http.get<any[]>('/api/cart').subscribe({
      next: (cartLines) => {
        this.cartLines = cartLines || [];
        this.cartSubject.next([...this.cartLines]);
      },
      error: (err) => {
        console.error('Error fetching cart lines:', err);
      }
    });
  }

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
    this.cartSubject.next([...this.cartLines]);

    // Send update request to server
    this.http.post<any>('/api/cart/update', payload).subscribe({
      next: (serverCartLine) => {
        const index = this.cartLines.findIndex(line => line.isp.id === isp.id);
        const serverIsp = serverCartLine.isp ? JSON.parse(JSON.stringify(serverCartLine.isp)) : null;
        const isRemoval = serverCartLine.qty === 0;

        if (index !== -1) {
          if (isRemoval) {
            this.cartLines.splice(index, 1);
          } else {
            this.cartLines[index] = {
              cartLineID: serverCartLine.cartLineID,
              qty: serverCartLine.qty,
              isp: serverIsp
            };
          }
        } else if (!isRemoval) {
          this.cartLines.push({
            cartLineID: serverCartLine.cartLineID,
            qty: serverCartLine.qty,
            isp: serverIsp
          });
        }
        this.cartSubject.next([...this.cartLines]);
      },
      error: (err) => {
        console.error('Error updating cart on server:', err);
        // Fallback: sync with server
        this.fetchCart();
      }
    });
  }

  clearCart() {
    this.cartLines = [];
    this.cartSubject.next([...this.cartLines]);
  }

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
