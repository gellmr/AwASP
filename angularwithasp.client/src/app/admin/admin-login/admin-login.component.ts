import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../../services/cart.service';
import { GoogleLoginCompComponent } from '../google-login-comp/google-login-comp.component';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [CommonModule, FormsModule, GoogleLoginCompComponent],
  templateUrl: './admin-login.component.html',
  styleUrl: './admin-login.component.css'
})
export class AdminLoginComponent implements OnInit {
  isLoading = false;
  error: string | null = null;

  // User will type a value into these at runtime
  vipUserName = '';
  vipPassword = '';

  private http = inject(HttpClient);
  private router = inject(Router);
  private cartService = inject(CartService);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.error = null;
  }

  loginClick() {
    this.isLoading = true;
    this.error = null;
    this.cdr.markForCheck();

    const payload = { username: this.vipUserName, password: this.vipPassword };

    this.http.post<any>('/api/admin-login', payload).subscribe({
      next: (userData) => {
        console.log('Login success. User data received:', userData);
        this.cartService.setUser(userData);
        this.isLoading = false;
        this.router.navigate(['/admin/orders/1']);
      },
      error: (err) => {
        console.error('Login failed:', err);
        this.error = err.error?.message || 'Incorrect username or password';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  handleKeyDown(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      event.preventDefault();
      this.loginClick();
    }
  }
}
