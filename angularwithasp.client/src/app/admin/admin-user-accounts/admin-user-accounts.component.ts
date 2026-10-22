import { Component, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SearchInputComponent }   from '../../search/search-input/search-input.component';
import { CartService } from '../../services/cart.service';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { DragDropUserPicModalComponent } from '../../drag-drop-user-pic-modal/drag-drop-user-pic-modal.component';
import { PagLinksComponent } from '../admin-orders-pag/pag-links.component';
import { PagedSearchComponent }   from '../paged-search.component';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-admin-user-accounts',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    SearchInputComponent,
    AdminTitleBarComponent,
    PagLinksComponent,
    DragDropUserPicModalComponent
  ],
  templateUrl: './admin-user-accounts.component.html',
  styleUrl: './admin-user-accounts.component.css'
})
export class AdminUserAccountsComponent extends PagedSearchComponent
{
  @ViewChild(DragDropUserPicModalComponent) modal!: DragDropUserPicModalComponent;

  userAccounts: any[] = [];
  
  public cartService = inject(CartService);

  override getRoutePrefix(): string[] {
    return ['/admin/useraccounts'];
  }

  // Implement the API call specific to orders
  override fetchData(page: number, search: string): Observable<any> {
    let query = `?ps=${this.rowsPerPage}`;
    if (search && search.trim()) {
      query += `&bs=${encodeURIComponent(search.trim())}`;
    }
    return this.http.get<any>(`/api/admin-useraccounts/${page}${query}`);
  }

  // Implement how the response data is parsed and assigned
  override processResponse(data: any): void {
    this.userAccounts = data.users || [];
    this.totResults = data.totalRows || 0;
    this.totPages = Math.ceil(this.totResults / this.rowsPerPage);
  }
  
  get myUserId(): string | undefined {
    return this.cartService.user ? this.cartService.user.appUserId : undefined;
  }

  // Returns only the N items for the current page
  get paginatedUsers(): any[] {
    return this.userAccounts;
  }

  // eg 1,2,3 for the record in userAccounts
  getSequenceNumber(index: number): number {
    return (this.currPage - 1) * this.rowsPerPage + index + 1;
  }

  handleClickPhoto(idval: string, usertype: string) {
    if (this.modal) {
      this.modal.showModal(idval, usertype);
    }
  }

  handleModalSuccess(event: { idval: string, picture: string, usertype: string }) {
    this.userAccounts = this.userAccounts.map(row => {
      const isMatch = (event.usertype === 'guest' && row.guestID === event.idval) || (row.id === event.idval);
      if (isMatch) {
         return { ...row, picture: event.picture };
      }
      return row;
    });
    this.cdr.markForCheck();
  }

  getUserType(user: any): string {
    return user.guestID !== null ? 'guest' : 'user';
  }

  getIdVal(user: any): string {
    return user.guestID !== null ? user.guestID : user.id;
  }

  isCurrentUser(user: any): boolean {
    const uid = this.myUserId;
    return !!(uid && uid === user.id);
  }
}

