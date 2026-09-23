import { Component, OnInit, ViewChild, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../services/cart.service';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { BackLinkComponent } from '../../shop/back-link/back-link.component';
import { DragDropUserPicModalComponent } from '../../drag-drop-user-pic-modal/drag-drop-user-pic-modal.component';

@Component({
  selector: 'app-admin-user-edit',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    AdminTitleBarComponent,
    BackLinkComponent,
    DragDropUserPicModalComponent
  ],
  templateUrl: './admin-user-edit.component.html',
  styleUrl: './admin-user-edit.component.css'
})
export class AdminUserEditComponent implements OnInit {
  @ViewChild(DragDropUserPicModalComponent) modal!: DragDropUserPicModalComponent;

  usertype: string = '';
  idval: string = '';
  userAccount: any = null;
  isLoading = true;
  error: string | null = null;

  private route = inject(ActivatedRoute);
  private http = inject(HttpClient);
  private router = inject(Router);
  public cartService = inject(CartService);
  private cdr = inject(ChangeDetectorRef);

  get myUserId(): string | undefined {
    return this.cartService.user ? this.cartService.user.appUserId : undefined;
  }

  get isCurrentUser(): boolean {
    const myUid = this.myUserId;
    return !!(myUid && this.userAccount && myUid === this.userAccount.id);
  }

  get toPayments(): string {
    return this.userAccount?.id ? `/admin/user/${this.userAccount.id}/payments` : '';
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.idval = params.get('id') || '';
      this.usertype = this.router.url.includes('/admin/guest/') ? 'guest' : 'user';
      this.fetchAccount();
    });
  }

  fetchAccount() {
    this.isLoading = true;
    this.error = null;
    this.cdr.markForCheck();

    const path = (this.usertype === 'guest') ? `/api/admin-guest-edit/${this.idval}` : `/api/admin-user-edit/${this.idval}`;
    this.http.get<any>(path).subscribe({
      next: (data) => {
        console.log('Account fetched:', data);
        this.userAccount = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Fetch account failed:', err);
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

  handleClickPhoto() {
    if (this.modal) {
      this.modal.showModal(this.idval, this.usertype);
    }
  }

  handleModalSuccess(event: { idval: string, picture: string, usertype: string }) {
    if (this.userAccount) {
      this.userAccount = { ...this.userAccount, picture: event.picture };
      this.cdr.markForCheck();
    }
  }

  onFieldChange(field: string, event: Event) {
    if (!this.userAccount) return;
    const input = event.target as HTMLInputElement;
    const value = input.value;

    // Optimistically update
    this.userAccount = { ...this.userAccount, [field]: value };
    this.cdr.markForCheck();

    // Call API
    const isGuest = this.userAccount.guestID !== null;
    const url = isGuest ? '/api/admin-guest-update' : '/api/admin-user-update';

    this.http.post<any>(url, this.userAccount).subscribe({
      next: (res) => {
        console.log('Update success:', res.message);
        this.userAccount = res.persist;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Update failed:', err);
        if (err.error?.revert) {
          this.userAccount = err.error.revert;
        }
        this.cdr.markForCheck();
      }
    });
  }
}

