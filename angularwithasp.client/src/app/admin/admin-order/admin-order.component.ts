import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { AdminTitleBarComponent } from '../admin-title-bar/admin-title-bar.component';
import { BackLinkComponent } from '../../shop/back-link/back-link.component';

@Component({
  selector: 'app-admin-order',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminTitleBarComponent, BackLinkComponent],
  templateUrl: './admin-order.component.html',
  styleUrl: './admin-order.component.css'
})
export class AdminOrderComponent implements OnInit, OnDestroy {
  orderid: string = '';
  myOrd: any = null;
  isLoading: boolean = false;
  error: string | null = null;
  isUserDetailsCollapsed: boolean = false;

  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);
  private sub = new Subscription();

  ngOnInit() {
    this.sub.add(this.route.paramMap.subscribe(params => {
      this.orderid = params.get('id') || '';
      this.fetchOrder();
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  fetchOrder() {
    this.isLoading = true;
    this.error = null;
    this.cdr.markForCheck();

    const payload = { orderid: parseInt(this.orderid, 10) };
    this.http.post<any>('/api/myorders/fetch-order', payload).subscribe({
      next: (res) => {
        this.myOrd = res.order;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err) => {
        this.isLoading = false;
        if (err.status === 401) {
          console.log("Admin not logged in. Redirect to login...");
          this.router.navigate(['/admin']);
        } else {
          this.error = err.error?.message || 'Something went wrong';
        }
        this.cdr.markForCheck();
      }
    });
  }

  toggleUserDetails() {
    this.isUserDetailsCollapsed = !this.isUserDetailsCollapsed;
    this.cdr.markForCheck();
  }

  get fullName(): string {
    if (!this.myOrd) return '';
    const guestFullName = this.myOrd.guest ? this.myOrd.guest.fullName : null;
    return this.myOrd.appUser ? this.myOrd.appUser.fullName : guestFullName;
  }

  get payReceived(): number {
    return this.myOrd ? this.myOrd.orderPaymentsReceived : 0;
  }

  get payOutstand(): number {
    if (!this.myOrd) return 0;
    return this.myOrd.priceTotal - this.myOrd.orderPaymentsReceived;
  }

  get customerEditPath(): string {
    if (!this.myOrd) return '';
    const idSeg = this.myOrd.accountType === 'User' ? this.myOrd.userID : this.myOrd.guestID;
    const editPathSeg = this.myOrd.accountType === 'User' ? 'user' : 'guest';
    return `/admin/${editPathSeg}/${idSeg}/edit`;
  }

  addressSegment(seg: string, isFinal: boolean = false): string {
    if (seg === undefined || seg === null || seg.trim().length === 0) {
      return "";
    }
    return isFinal ? seg : seg + ", ";
  }

  oneLineAddress(address: any): string {
    if (!address) { return ""; }
    return (
      this.addressSegment(address.line1) +
      this.addressSegment(address.line2) +
      this.addressSegment(address.line3) +
      this.addressSegment(address.city) +
      this.addressSegment(address.state) +
      this.addressSegment(address.country) +
      this.addressSegment(address.zip, true)
    );
  }

  get shipAddy(): string {
    return this.myOrd ? this.oneLineAddress(this.myOrd.shipAddress) : '';
  }

  get billAddy(): string {
    if (!this.myOrd) return '';
    const ship = this.shipAddy;
    const bill = this.oneLineAddress(this.myOrd.billAddress);
    if (!bill) return '';
    return bill === ship ? '(same as shipping address)' : bill;
  }
}

