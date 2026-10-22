import { Component, Input, OnInit, ChangeDetectorRef, inject, OnDestroy, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, NavigationEnd, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { CartService } from '../services/cart.service';
import { HamburgerComponent } from './links/hamburger.component';

import { RespAdminLinksComponent } from './links/resp--admin-links';
import { RespShopLinksComponent } from './links/resp--shop-links';
import { RespLoginLinksComponent } from './links/resp--login-links';

@Component({
  selector: 'app-site-nav-bar',
  standalone: true,
  imports: [
    RouterModule,
    RespAdminLinksComponent,
    RespShopLinksComponent,
    HamburgerComponent,
    CommonModule,
    RespLoginLinksComponent
],
  templateUrl: './site-nav-bar.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrl: './site-nav-bar.component.css'
})
export class SiteNavBarComponent implements OnInit, OnDestroy {
  @Input() brandText: string = "SPORTS STORE";
  @Input() linkTo: string = "/";

  showCart = true;
  isCollapsed = true;
  cartQty = 0;
  ordersQty = 0;

  isLoginRoute = false; // true if we are on the login page
  withinAdmin = false;  // true if we are in admin section

  private router = inject(Router);
  private http = inject(HttpClient);
  private cdr = inject(ChangeDetectorRef);
  public cartService = inject(CartService);
  private sub = new Subscription();

  ngOnInit() {
    this.checkRoute(this.router.url);
    this.sub.add(this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: any) => {
      this.checkRoute(event.urlAfterRedirects);
    }));

    this.sub.add(this.cartService.cart$.subscribe(cartLines => {
      this.cartQty = cartLines.reduce((sum, row) => sum + row.qty, 0);
      this.cdr.markForCheck();
    }));

    this.sub.add(this.cartService.orders$.subscribe(orders => {
      this.ordersQty = orders.length;
      this.cdr.markForCheck();
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  private checkRoute(url: string) {
    this.showCart = url === '/' || url.startsWith('/category') || url === '/cart';
    this.withinAdmin = url.indexOf('/admin') !== -1;
    this.isLoginRoute = url === '/admin';
    this.cdr.markForCheck();
  }

  logoutAdmin(event: MouseEvent) {
    event.preventDefault();
    this.http.post('/api/admin-logout', {}).subscribe({
      next: () => {
        console.info("Logged out successfully.");
        this.cartService.setUser(null); this.cdr.detectChanges(); // Sync immediately to avoid a collision.
        this.router.navigate(['/logoutsuccess']);
      },
      error: (err) => {
        console.error('Logout failed:', err);
        this.cartService.setUser(null); this.cdr.detectChanges(); // Sync immediately to avoid a collision.
        this.router.navigate(['/']);
      }
    });
  }
}
