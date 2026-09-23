import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart-product',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart-product.component.html',
  styleUrl: './cart-product.component.css'
})
export class CartProductComponent {
  @Input() cartLine: any;

  private cartService = inject(CartService);

  remove() {
    this.cartService.updateCartQuantity(this.cartLine.cartLineID, 0, this.cartLine.isp);
  }

  increment() {
    const newQty = this.cartLine.qty + 1;
    this.cartService.updateCartQuantity(this.cartLine.cartLineID, newQty, this.cartLine.isp);
  }

  decrement() {
    if (this.cartLine.qty > 0) {
      const newQty = this.cartLine.qty - 1;
      this.cartService.updateCartQuantity(this.cartLine.cartLineID, newQty, this.cartLine.isp);
    }
  }
}
