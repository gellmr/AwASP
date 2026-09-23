import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SiteNavBarComponent } from '../../nav/site-nav-bar/site-nav-bar.component';
import { CategoriesMenuComponent } from '../../shop/categories-menu/categories-menu.component';
import { FooterComponent } from '../../shop/footer/footer.component';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-shop-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, SiteNavBarComponent, CategoriesMenuComponent, FooterComponent],
  templateUrl: './shop-layout.component.html',
  styleUrl: './shop-layout.component.css'
})
export class ShopLayoutComponent implements OnInit {
  backCss: string = 'soccerBaseBg soccerBg1';
  private cartService = inject(CartService);

  get isGuestReady(): string {
    return this.cartService.getGuestId() ? 'true' : 'false';
  }

  ngOnInit() {
    this.cartService.init();
  }
}
