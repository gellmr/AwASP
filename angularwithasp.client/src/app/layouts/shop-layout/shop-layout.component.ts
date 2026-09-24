import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { SiteNavBarComponent } from '../../nav/site-nav-bar/site-nav-bar.component';
import { CategoriesMenuComponent } from '../../shop/categories-menu/categories-menu.component';
import { FooterComponent } from '../../shop/footer/footer.component';
import { CartService } from '../../services/cart.service';
import { Subscription, combineLatest } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-shop-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, SiteNavBarComponent, CategoriesMenuComponent, FooterComponent],
  templateUrl: './shop-layout.component.html',
  styleUrl: './shop-layout.component.css'
})
export class ShopLayoutComponent implements OnInit, OnDestroy {
  backCss: string = 'soccerBaseBg soccerBg1';
  bgTransparentClass: string = 'shopLayoutTransparent bgRegularTransparent';
  
  private cartService = inject(CartService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);
  private subscription = new Subscription();

  get isGuestReady(): string {
    return this.cartService.getGuestId() ? 'true' : 'false';
  }

  ngOnInit() {
    this.cartService.init();

    // 1. Subscribe to Router NavigationEnd to update styling based on active URL
    this.subscription.add(
      this.router.events.pipe(
        filter(event => event instanceof NavigationEnd)
      ).subscribe((event: any) => {
        this.updateBackgroundAndTransparent(event.urlAfterRedirects || event.url);
      })
    );

    // 2. Subscribe to Cart and Orders changes to keep transparent overlay reactive
    this.subscription.add(
      combineLatest([
        this.cartService.cart$,
        this.cartService.orders$
      ]).subscribe(() => {
        this.updateBackgroundAndTransparent(this.router.url);
      })
    );

    // Initial load
    this.updateBackgroundAndTransparent(this.router.url);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  updateBackgroundAndTransparent(url: string) {
    if (!url) {
      return;
    }

    // A. Parse category for page background image
    let category = '';
    const match = url.match(/\/category\/([^/]+)/);
    if (match) {
      category = match[1];
    }

    let css = 'soccerBg1';
    switch (category) {
      case 'soccer': css = 'soccerBg2'; break;
      case 'chess': css = 'chessBg'; break;
      case 'waterSport': css = 'kayakBg'; break;
    }
    this.backCss = 'soccerBaseBg ' + css;

    // B. Calculate transparency overlay class matching React logic
    const pathname = url.split('?')[0]; // Strip query parameters
    let transparentClass = 'shopLayoutTransparent bgRegularTransparent';
    
    const cartLines = this.cartService.getCartLines();
    const cartQty = cartLines.reduce((sum, row) => sum + row.qty, 0);
    
    const orders = this.cartService.getOrders();
    const ordQty = orders.reduce((sum, row) => sum + row.qty, 0);

    if (pathname === '/myorders' && ordQty === 0) {
      transparentClass = 'shopLayoutTransparent bgOrderFullTransparent';
    }
    if (pathname === '/cart' && cartQty === 0) {
      transparentClass = 'shopLayoutTransparent bgCartFullTransparent';
    }
    if (pathname === '/checkout') {
      transparentClass = 'shopLayoutTransparent bgStrongTransparent';
    }
    if (pathname.includes('/myorders/')) {
      transparentClass = 'shopLayoutTransparent bgStrongTransparent';
    }
    
    this.bgTransparentClass = transparentClass;
    this.cdr.markForCheck();
  }
}
