import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-checkout-success',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './checkout-success.component.html',
  styleUrl: './checkout-success.component.css'
})
export class CheckoutSuccessComponent implements OnInit {
  private cartService = inject(CartService);

  ngOnInit() {
    this.cartService.fetchMyOrders().subscribe({
      error: (err) => console.error('Error refreshing orders after checkout:', err)
    });
  }
}

