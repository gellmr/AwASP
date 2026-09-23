import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { MyOrdersShowAccountInfoComponent } from '../my-orders-show-account-info/my-orders-show-account-info.component';
import { BackLinkComponent } from '../back-link/back-link.component';

@Component({
  selector: 'app-my-order-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, MyOrdersShowAccountInfoComponent, BackLinkComponent],
  templateUrl: './my-order-detail.component.html',
  styleUrl: './my-order-detail.component.css'
})
export class MyOrderDetailComponent implements OnInit, OnDestroy {
  orderid: string = '';
  ord: any = null;

  isLoading = true;
  error: any = null;

  fullname = '';
  email = '';
  accType = 'Guest';
  idval = '';
  devMode = true;

  private cartService = inject(CartService);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);
  private sub = new Subscription();

  ngOnInit() {
    this.sub.add(this.route.paramMap.subscribe(params => {
      this.orderid = params.get('id') || '';
      
      this.loadOrder();
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  loadOrder() {
    this.isLoading = true;

    // Get guest info
    const guest = this.cartService.guest;
    if (guest) {
      this.fullname = guest.fullname || guest.fullName || '';
      this.email = guest.email || '';
      this.accType = 'Guest';
      this.idval = guest.id;
    }

    this.sub.add(this.cartService.orders$.subscribe(orders => {
      if (orders && orders.length > 0) {
        this.ord = orders.find((o: any) => o.id.toString() === this.orderid);
        this.updateAccountInfoFromOrder();
        this.isLoading = false;
        this.cdr.markForCheck();
      } else {
        // Fetch orders if they are not loaded yet
        this.cartService.fetchMyOrders().subscribe({
          next: (res) => {
            const fetchedOrders = res.rows || [];
            this.ord = fetchedOrders.find((o: any) => o.id.toString() === this.orderid);
            this.updateAccountInfoFromOrder();
            this.isLoading = false;
            this.cdr.markForCheck();
          },
          error: (err) => {
            this.error = err;
            this.isLoading = false;
            this.cdr.markForCheck();
          }
        });
      }
    }));
  }

  updateAccountInfoFromOrder() {
    if (this.ord) {
      if (this.ord.guest) {
        this.fullname = this.ord.guest.fullName || this.ord.guest.fullname || '';
        this.email = this.ord.guest.email || '';
        this.idval = this.ord.guest.id || '';
        this.accType = 'Guest';

        this.cartService.guest = {
          id: this.idval,
          fullname: this.fullname,
          email: this.email,
          firstname: this.ord.guest.firstName || this.ord.guest.firstname || '',
          lastname: this.ord.guest.lastName || this.ord.guest.lastname || ''
        };
      } else if (this.ord.appUser) {
        this.fullname = this.ord.appUser.fullName || this.ord.appUser.fullname || '';
        this.email = this.ord.appUser.email || '';
        this.idval = this.ord.appUser.id || '';
        this.accType = this.ord.accountType || 'User';
      }
    }
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

  getShipAddress(): string {
    if (!this.ord) { return ''; }
    return this.ord.shippingAddress ? this.ord.shippingAddress : this.oneLineAddress(this.ord.shipAddress);
  }

  getBillAddress(): string {
    if (!this.ord) { return ''; }
    const shipAddy = this.getShipAddress();
    const bill = this.ord.billingAddress ? this.ord.billingAddress : this.oneLineAddress(this.ord.billAddress);
    return bill === shipAddy ? "(same as shipping address)" : bill;
  }

  displayDateObj(inString: string) {
    const dateDisplayFormat: Intl.DateTimeFormatOptions = {
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: true,
      weekday: 'long',
      day: 'numeric',
      month: 'long',
      year: 'numeric',
      timeZoneName: 'longOffset'
    };
    try {
      const formattedString = new Date(inString).toLocaleDateString('en-US', dateDisplayFormat);
      const splitRes = formattedString.split("GMT");
      const datePart = splitRes[0] || '';
      const tzPart = splitRes[1] || '';
      return { datePart, tzPart: tzPart ? 'GMT' + tzPart : '' };
    } catch (e) {
      return { datePart: inString, tzPart: '' };
    }
  }
}
