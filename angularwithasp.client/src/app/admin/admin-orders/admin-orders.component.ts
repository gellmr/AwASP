import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { SearchInputComponent } from '../../search/search-input/search-input.component';
import { PaginationLinksComponent } from '../../shop/pagination-links/pagination-links.component';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    SearchInputComponent,
    PaginationLinksComponent,
    AdminTitleBarComponent
  ],
  templateUrl: './admin-orders.component.html',
  styleUrl: './admin-orders.component.css'
})
export class AdminOrdersComponent implements OnInit, OnDestroy {
  adminOrders: any[] = [];
  error: string | null = null;
  isLoading: boolean = false;

  pageSize: number = 12;
  currPage: number = 1;
  totResults: number = 0;
  numPages: number = 0;
  backlogSearch: string = "";

  private http = inject(HttpClient);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);
  private sub = new Subscription();

  ngOnInit() {
    this.sub.add(this.route.paramMap.subscribe(params => {
      const pageStr = params.get('page');
      this.currPage = pageStr ? parseInt(pageStr, 10) : 1;
      this.fetchAdminOrders();
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  fetchAdminOrders() {
    this.isLoading = true;
    this.error = null;
    this.cdr.markForCheck();

    let query = `?ps=${this.pageSize}`;
    if (this.backlogSearch && this.backlogSearch.trim()) {
      query += `&bs=${encodeURIComponent(this.backlogSearch.trim())}`;
    }

    const url = `/api/admin-orders/${this.currPage}${query}`;
    this.http.get<any>(url).subscribe({
      next: (data) => {
        this.adminOrders = data.orders || [];
        this.totResults = this.adminOrders.length > 0 ? this.adminOrders[0].totalRows : 0;
        this.numPages = Math.ceil(this.totResults / this.pageSize);
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 401) {
          console.log("Admin not logged in. Redirect to login...");
          this.router.navigate(['/admin']);
        } else {
          this.error = err.error?.errMessage || "Something went wrong";
        }
        this.cdr.markForCheck();
      }
    });
  }

  handleSearchChange(str: string) {
    this.backlogSearch = str;
    if (this.currPage !== 1) {
      this.router.navigate(['/admin/orders', 1]);
    } else {
      this.fetchAdminOrders();
    }
  }

  handleClickBacklogRow(orderid: number) {
    this.router.navigate(['/admin/order', orderid]);
  }
}

