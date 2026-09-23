import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { BackLinkComponent } from '../../shop/back-link/back-link.component';

@Component({
  selector: 'app-admin-user-orders',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    AdminTitleBarComponent,
    BackLinkComponent
  ],
  templateUrl: './admin-user-orders.component.html',
  styleUrl: './admin-user-orders.component.css'
})
export class AdminUserOrdersComponent implements OnInit {
  usertype: string = '';
  idval: string = '';
  fullname: string = '';
  userOrders: any[] = [];
  isLoading = true;
  error: string | null = null;

  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  get userDisplayName(): string {
    return "Orders for " + this.fullname;
  }

  get gotOrders(): boolean {
    return Array.isArray(this.userOrders) && this.userOrders.length > 0;
  }

  get whiteBackStyle(): string {
    return this.gotOrders ? 'clearBack' : 'whiteBack';
  }

  get linerStyle(): string {
    return this.gotOrders ? 'adminUserOrdersLiner whiteLiner' : 'adminUserOrdersLiner';
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.idval = params.get('id') || '';
      this.usertype = this.router.url.includes('/admin/guest/') ? 'guest' : 'user';
      this.fetchOrders();
    });
  }

  fetchOrders() {
    this.isLoading = true;
    this.error = null;
    this.cdr.markForCheck();

    const url = `/api/admin-user-orders?idval=${this.idval}&usertype=${this.usertype}`;
    this.http.get<any>(url).subscribe({
      next: (response) => {
        console.log('Orders fetched:', response);
        this.fullname = response.fullName || '';
        this.userOrders = response.orders || [];
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Fetch orders failed:', err);
        this.isLoading = false;
        if (err.status === 401) {
          this.router.navigate(['/admin']);
        } else {
          this.error = err.error?.message || err.message || 'Something went wrong';
        }
        this.cdr.markForCheck();
      }
    });
  }
}

