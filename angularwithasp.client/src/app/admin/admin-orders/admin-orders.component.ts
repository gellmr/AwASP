import { Component, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SearchInputComponent }   from '../../search/search-input/search-input.component';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { PagLinksComponent }      from '../admin-orders-pag/pag-links.component';
import { PagedSearchComponent }   from '../paged-search.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    SearchInputComponent,
    PagLinksComponent,
    AdminTitleBarComponent
  ],
  encapsulation: ViewEncapsulation.None,
  templateUrl: './admin-orders.component.html',
  styleUrl: './admin-orders.component.css'
})
export class AdminOrdersComponent extends PagedSearchComponent
{
  adminOrders: any[] = [];

  // Define the base route prefix used when resetting to page 1 on search
  override getRoutePrefix(): string[] {
    return ['/admin/orders'];
  }
  
  // Implement the API call specific to orders
  override fetchData(page: number, search: string): Observable<any> {
    let query = `?ps=${this.rowsPerPage}`;
    if (search && search.trim()) {
      query += `&bs=${encodeURIComponent(search.trim())}`;
    }
    return this.http.get<any>(`/api/admin-orders/${page}${query}`);
  }

  // Implement how the response data is parsed and assigned
  override processResponse(data: any): void {
    this.adminOrders = data.orders || [];
    this.totResults = this.adminOrders.length > 0 ? this.adminOrders[0].totalRows : 0;
    this.totPages = Math.ceil(this.totResults / this.rowsPerPage);
  }
  
  handleClickedRow(orderid: number) {
    this.router.navigate(['/admin/order', orderid]);
  }
}
