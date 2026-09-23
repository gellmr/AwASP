import { Component, OnInit, ViewChild, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { DragDropUserPicModalComponent } from '../../drag-drop-user-pic-modal/drag-drop-user-pic-modal.component';

@Component({
  selector: 'app-admin-user-accounts',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    AdminTitleBarComponent,
    DragDropUserPicModalComponent
  ],
  templateUrl: './admin-user-accounts.component.html',
  styleUrl: './admin-user-accounts.component.css'
})
export class AdminUserAccountsComponent implements OnInit {
  @ViewChild(DragDropUserPicModalComponent) modal!: DragDropUserPicModalComponent;

  userAccounts: any[] = [];
  isLoading = false;
  error: string | null = null;

  private http = inject(HttpClient);
  private router = inject(Router);
  public cartService = inject(CartService);
  private cdr = inject(ChangeDetectorRef);

  get myUserId(): string | undefined {
    return this.cartService.user ? this.cartService.user.appUserId : undefined;
  }

  ngOnInit() {
    this.fetchAdminUserAccs();
  }

  fetchAdminUserAccs() {
    this.isLoading = true;
    this.error = null;
    this.cdr.markForCheck();

    const url = '/api/admin-useraccounts';
    this.http.get<any[]>(url).subscribe({
      next: (data) => {
        console.log('User accounts data fetched:', data);
        this.userAccounts = data || [];
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Request failed:', err);
        this.isLoading = false;
        if (err.status === 401) {
          console.log("Admin not logged in. Redirect to login...");
          this.router.navigate(['/admin']);
        } else {
          this.error = err.error?.message || err.message || "Something went wrong";
        }
        this.cdr.markForCheck();
      }
    });
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

