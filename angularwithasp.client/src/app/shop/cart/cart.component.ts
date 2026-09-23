import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { CartProductComponent } from '../cart-product/cart-product.component';
import { CartSummaryLineComponent } from '../cart-summary-line/cart-summary-line.component';
import { ProceedCheckoutBtnComponent } from '../proceed-checkout-btn/proceed-checkout-btn.component';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, CartProductComponent, CartSummaryLineComponent, ProceedCheckoutBtnComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css'
})
export class CartComponent implements OnInit, OnDestroy {
  cartLines: any[] = [];
  gotItems = false;
  showTopCheckoutBtn = false;
  totalQty = 0;
  totalPrice = 0;
  cartLen = 'Cart is Empty';

  private cartService = inject(CartService);
  private cdr = inject(ChangeDetectorRef);
  private sub = new Subscription();

  ngOnInit() {
    this.sub = this.cartService.cart$.subscribe(cartLines => {
      this.cartLines = cartLines;
      this.gotItems = cartLines.length > 0;
      this.showTopCheckoutBtn = cartLines.length > 5;
      
      this.totalQty = cartLines.reduce((sum, row) => sum + row.qty, 0);
      this.totalPrice = cartLines.reduce((sum, row) => sum + (row.isp.price * row.qty), 0);
      this.cartLen = this.totalQty > 0 ? 'My Cart:' : 'Cart is Empty';
      
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  clear() {
    this.cartService.clearCartOnServer();
  }
}
