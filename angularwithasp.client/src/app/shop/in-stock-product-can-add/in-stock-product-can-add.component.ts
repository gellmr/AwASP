import { Component, Input, ViewEncapsulation, computed, inject} from '@angular/core';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-in-stock-product-can-add',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './in-stock-product-can-add.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrl: './in-stock-product-can-add.component.css'
})
export class InStockProductCanAddComponent
{
  @Input() product: any;
  
  private cartService = inject(CartService);

  // Convert the RxJS cart$ stream into a Signal.
  private cartLines = toSignal(this.cartService.cart$, {initialValue: []} );

  // Signal holding our in stock product, as seen in the cartLines
  private cartProd = computed(() => this.cartLines().find((line: any) => line.isp.id === this.product?.id));

  // Signal holding the current cart qty of this in stock product.
  qtyInCart = computed(() => this.cartProd()?.qty ?? 0);

  // Signal holding the cartLineID of the in stock product.
  cartLineID = computed(() => this.cartProd()?.cartLineID ?? null);

  incrementQty() {
    const newQty = this.qtyInCart() + 1;
    this.cartService.updateCartQuantity(this.cartLineID(), newQty, this.product);
  }

  decrementQty() {
    if (this.qtyInCart() > 0) {
      const newQty = this.qtyInCart() - 1;
      this.cartService.updateCartQuantity(this.cartLineID(), newQty, this.product);
    }
  }
}
