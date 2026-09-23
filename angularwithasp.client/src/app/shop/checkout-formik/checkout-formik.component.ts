import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-checkout-formik',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './checkout-formik.component.html',
  styleUrl: './checkout-formik.component.css'
})
export class CheckoutFormikComponent implements OnInit {
  checkoutForm!: FormGroup;
  autoFillIdx = 0;

  private fb = inject(FormBuilder);
  private cartService = inject(CartService);
  private router = inject(Router);
  private http = inject(HttpClient);

  autoVal = [
    {
      firstName : "John",
      lastName  : "Doe",
      shipLine1   : "123 River Gum Way",
      shipLine2   : "Unit 10/150, Third Floor",
      shipLine3   : "The Tall Apartment Building (Inc)",
      shipCity      : "SpringField",
      shipState     : "WA",
      shipCountry   : "Australia",
      shipZip       : "6525",
      shipEmail : "john@example.com",
    },
    {
      firstName : "Eliza",
      lastName  : "Parks",
      shipLine1:  "90 Taylors Rd",
      shipLine2   : "",
      shipLine3   : "",
      shipCity      : "Keilor Downs",
      shipState     : "VIC",
      shipCountry   : "Australia",
      shipZip       : "3038",
      shipEmail : "eparks@research.sportcom",
    },
    {
      firstName : "Arnold",
      lastName  : "Mosley",
      shipLine1   : "64 Orange Grove Rd",
      shipLine2   : "",
      shipLine3   : "",
      shipCity      : "Liverpool",
      shipState     : "NSW",
      shipCountry   : "Australia",
      shipZip       : "2170",
      shipEmail : "mosley@distantsound.net",
    }
  ];

  ngOnInit() {
    const guest = this.cartService.guest;
    const firstName = guest ? guest.firstname : '';
    const lastName = guest ? guest.lastname : '';
    const email = guest ? guest.email : '';

    this.checkoutForm = this.fb.group({
      firstName: [firstName, [Validators.required, Validators.minLength(2)]],
      lastName: [lastName, [Validators.minLength(2)]],
      shipLine1: ['', [Validators.required, Validators.minLength(2)]],
      shipLine2: [''],
      shipLine3: [''],
      shipCity: ['', [Validators.required, Validators.minLength(2)]],
      shipState: ['', [Validators.required, Validators.minLength(2)]],
      shipCountry: ['', [Validators.required, Validators.minLength(2)]],
      shipZip: ['', [Validators.required, Validators.minLength(4)]],
      shipEmail: [email, [Validators.required, Validators.email]]
    });
  }

  get f() {
    return this.checkoutForm.controls;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.checkoutForm.get(fieldName);
    return !!(field && field.invalid && (field.touched || field.dirty));
  }

  autoFill() {
    const vals = this.autoVal[this.autoFillIdx];
    this.checkoutForm.patchValue(vals);
    this.autoFillIdx = (this.autoFillIdx + 1) % this.autoVal.length;
  }

  onSubmit() {
    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    const values = this.checkoutForm.value;
    const cart = this.cartService.getCartLines();
    const jsonData = { cart, ...values };

    this.http.post('/api/checkout/submit', jsonData).subscribe({
      next: () => {
        this.cartService.clearCart();
        this.router.navigate(['/checkoutsuccess']);
      },
      error: (err) => {
        console.error('Error submitting checkout:', err);
      }
    });
  }
}
