import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Subscription } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { MyOrdersShowAccountInfoComponent } from '../my-orders-show-account-info/my-orders-show-account-info.component';
import { PaginationLinksComponent } from '../pagination-links/pagination-links.component';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [CommonModule, RouterModule, MyOrdersShowAccountInfoComponent, PaginationLinksComponent],
  templateUrl: './my-orders.component.html',
  styleUrl: './my-orders.component.css'
})
export class MyOrdersComponent implements OnInit, OnDestroy {
  orders: any[] = [];
  ordersThisPage: any[] = [];
  isLoading = true;
  error: any = null;

  currPage = 1;
  numPages = 1;
  myRoute = '/myorders/';

  fullname = '';
  email = '';
  accType = 'Guest';
  idval = '';
  devMode = true;

  emptyMsgText1 = '(None at the moment)';
  emptyMsgText2 = '';

  private cartService = inject(CartService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);
  private sub = new Subscription();

  ngOnInit() {
    this.sub.add(this.route.paramMap.subscribe(params => {
      const pageParam = params.get('page');
      this.currPage = pageParam ? parseInt(pageParam, 10) : 1;
      
      this.fetchOrders();
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  fetchOrders() {
    this.isLoading = true;

    const guest = this.cartService.guest;
    if (guest) {
      this.fullname = guest.fullname;
      this.email = guest.email;
      this.accType = 'Guest';
      this.idval = guest.id;
    }

    this.cartService.fetchMyOrders().subscribe({
      next: (res) => {
        this.orders = res.rows || [];

        const ordersPerPage = 3;
        const ordersCount = this.orders.length;
        const wholePages = Math.floor(ordersCount / ordersPerPage);
        const extraLines = ordersCount % ordersPerPage;
        const extraPage = (extraLines === 0 ? 0 : 1);
        this.numPages = wholePages + extraPage || 1;

        const pageIntP = (this.currPage > this.numPages) ? this.numPages : this.currPage;
        const pageIdx = pageIntP - 1;
        const startIdx = ordersPerPage * pageIdx;
        const endIdx = startIdx + ordersPerPage;

        this.ordersThisPage = this.orders.slice(startIdx, endIdx);
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

  getShipAddress(ord: any): string {
    return ord.shippingAddress ? ord.shippingAddress : this.oneLineAddress(ord.shipAddress);
  }

  getBillAddress(ord: any): string {
    const shipAddy = this.getShipAddress(ord);
    const bill = ord.billingAddress ? ord.billingAddress : this.oneLineAddress(ord.billAddress);
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
