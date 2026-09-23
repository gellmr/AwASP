import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    AdminTitleBarComponent
  ],
  templateUrl: './admin-products.component.html',
  styleUrl: './admin-products.component.css'
})
export class AdminProductsComponent implements OnInit {
  adminProducts: any[] = [];
  isLoading = false;
  error: string | null = null;

  private http = inject(HttpClient);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  ngOnInit() {
    this.fetchAdminProducts();
  }

  fetchAdminProducts() {
    this.isLoading = true;
    this.error = null;
    this.cdr.markForCheck();

    const url = '/api/admin-products';
    this.http.get<any[]>(url).subscribe({
      next: (data) => {
        console.log('Products fetched:', data);
        this.adminProducts = data || [];
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Fetch products failed:', err);
        this.isLoading = false;
        if (err.status === 401) {
          console.log("Admin not logged in. Redirect to login...");
          this.router.navigate(['/admin']);
        } else {
          this.error = err.error?.message || err.message || 'Something went wrong';
        }
        this.cdr.markForCheck();
      }
    });
  }
}

