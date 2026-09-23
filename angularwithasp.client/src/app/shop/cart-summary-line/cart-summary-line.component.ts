import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-cart-summary-line',
  standalone: true,
  templateUrl: './cart-summary-line.component.html',
  styleUrl: './cart-summary-line.component.css'
})
export class CartSummaryLineComponent {
  @Input() totalQuantity: number = 0;
  @Input() totalPrice: number = 0;
}
