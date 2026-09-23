import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CheckoutFormikComponent } from '../checkout-formik/checkout-formik.component';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, CheckoutFormikComponent],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent {
}
