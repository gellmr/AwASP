import { Component, Input, OnInit, ChangeDetectorRef, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, NavigationEnd, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { ResponsiveLinkComponent } from '../../nav/links/responsive-link.component';
import { VlComponent } from '../../nav/links/vl.component';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-site-nav-bar',
  standalone: true,
  imports: [CommonModule, RouterModule, ResponsiveLinkComponent, VlComponent],
  templateUrl: './site-nav-bar.component.html',
  styleUrl: './site-nav-bar.component.css'
})
export class SiteNavBarComponent implements OnInit, OnDestroy {
  @Input() brandText: string = "SPORTS STORE";
  @Input() linkTo: string = "/";

  showCart = true;
  isCollapsed = true;
  cartQty = 0;
  ordersQty = 0;
  withinAdmin = false;

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
    this.cdr.markForCheck();
  }

  toggleCollapse() {
    this.isCollapsed = !this.isCollapsed;
  }

  logoutAdmin(event: MouseEvent) {
    event.preventDefault();
    this.http.post('/api/admin-logout', {}).subscribe({
      next: () => {
        this.cartService.setUser(null);
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Logout failed:', err);
        this.cartService.setUser(null);
        this.router.navigate(['/']);
      }
    });
  }
}

