import { Component, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { SearchInputComponent }   from '../../search/search-input/search-input.component';
import { PagedSearchComponent }   from '../paged-search.component';
import { Observable } from 'rxjs';
import { PagLinksComponent }      from '../admin-orders-pag/pag-links.component';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    PagLinksComponent,
    SearchInputComponent,
    AdminTitleBarComponent
  ],
  templateUrl: './admin-products.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrl: './admin-products.component.css'
})
export class AdminProductsComponent extends PagedSearchComponent
{
  adminProducts: any[] = [];

  override getRoutePrefix(): string[] {
    return ['/admin/products'];
  }
  
  override fetchData(page: number, search: string): Observable<any> {
    let query = `?ps=${this.rowsPerPage}`;
    if (search && search.trim()) {
      query += `&bs=${encodeURIComponent(search.trim())}`;
    }
    return this.http.get<any>(`/api/admin-products/${page}${query}`);
  }

  override processResponse(data: any): void {
    this.adminProducts = data.prods || [];
    this.totResults = data.totalRows || 0;
    this.totPages = Math.ceil(this.totResults / this.rowsPerPage);
  }
  
  // Returns only the N items for the current page
  get paginatedRecords(): any[] {
    return this.adminProducts;
  }
}
