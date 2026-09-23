import { Component, AfterViewInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-google-login-comp',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './google-login-comp.component.html',
  styleUrl: './google-login-comp.component.css'
})
export class GoogleLoginCompComponent implements AfterViewInit {
  isLoading = false;
  error: string | null = null;

  private clientId: string = ''; // Starts empty. Formerly import.meta.env.VITE_GOOGLE_CLIENT_ID
  private http = inject(HttpClient);
  private router = inject(Router);
  private cartService = inject(CartService);
  private cdr = inject(ChangeDetectorRef);
  
  ngAfterViewInit() {
    // Fetch the client ID dynamically from the server
    this.http.get<{clientId: string}>('/api/EnvName/google-client-id').subscribe({
      next: (res) => {
        this.clientId = res.clientId;
        this.loadGoogleScript(); // Load and initialize Google button once we have the ID
      },
      error: (err) => console.error('Failed to load Google Client ID', err)
    });
  }

  loadGoogleScript() {
    if ((window as any).google?.accounts?.id) {
      this.initializeGoogleSignIn();
      return;
    }
    const script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    script.onload = () => {
      this.initializeGoogleSignIn();
    };
    document.head.appendChild(script);
  }

  initializeGoogleSignIn() {
    const google = (window as any).google;
    if (google && google.accounts && google.accounts.id) {
      google.accounts.id.initialize({
        client_id: this.clientId,
        callback: (response: any) => this.handleCredentialResponse(response)
      });
      google.accounts.id.renderButton(
        document.getElementById('google-btn-container'),
        { theme: 'outline', size: 'large' }
      );
    }
  }

  handleCredentialResponse(response: any) {
    this.isLoading = true;
    this.error = null;
    this.cdr.markForCheck();

    this.http.post<any>('/api/validate-google-token', response).subscribe({
      next: (userData) => {
        console.log('Google login success. User data received:', userData);
        this.cartService.setUser(userData);
        this.isLoading = false;
        this.router.navigate(['/admin/orders/1']);
      },
      error: (err) => {
        console.error('Google login failed:', err);
        this.error = err.error?.loginResult || 'Failed';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
