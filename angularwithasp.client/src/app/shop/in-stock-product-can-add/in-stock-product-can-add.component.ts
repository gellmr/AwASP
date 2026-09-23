import { Component, Input, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-in-stock-product-can-add',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './in-stock-product-can-add.component.html',
  styleUrl: './in-stock-product-can-add.component.css'
})
export class InStockProductCanAddComponent implements OnInit, OnDestroy {
  @Input() product: any;
  
  qtyInCart: number = 0;
  cartLineID: number | null = null;

  private cartService = inject(CartService);
  private cdr = inject(ChangeDetectorRef);
  private sub = new Subscription();

  ngOnInit() {
    this.sub = this.cartService.cart$.subscribe(cartLines => {
      const cartProd = cartLines.find((line: any) => line.isp.id === this.product.id);
      if (cartProd) {
        this.qtyInCart = cartProd.qty;
        this.cartLineID = cartProd.cartLineID;
      } else {
        this.qtyInCart = 0;
        this.cartLineID = null;
      }
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  incrementQty() {
    const newQty = this.qtyInCart + 1;
    this.cartService.updateCartQuantity(this.cartLineID, newQty, this.product);
  }

  decrementQty() {
    if (this.qtyInCart > 0) {
      const newQty = this.qtyInCart - 1;
      this.cartService.updateCartQuantity(this.cartLineID, newQty, this.product);
    }
  }
}
