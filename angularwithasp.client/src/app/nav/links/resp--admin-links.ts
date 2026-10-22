import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';

// Dedicated link components
import { RespLinkBackToShopComponent }       from './resp-link-back-to-shop.component';
import { RespLinkOrdersBacklogComponent }    from './resp-link-orders-backlog.component';
import { RespLinkCustomerAccountsComponent } from './resp-link-customer-accounts.component';
import { RespLinkProductsComponent }         from './resp-link-products.component';
import { RespLinkBiComponent }               from './resp-link-bi.component';
import { RespLinkSaleHistComponent }         from './resp-link-salehist.component';
import { LogoutLinkComponent } from './logout-link.component';

@Component({
  selector: 'app-resp-adminlinks',
  standalone: true,
  imports: [
    RespLinkBackToShopComponent,
    RespLinkOrdersBacklogComponent,
    RespLinkCustomerAccountsComponent,
    RespLinkProductsComponent,
    RespLinkBiComponent,
    RespLinkSaleHistComponent,
    LogoutLinkComponent,
    CommonModule
  ],
  encapsulation: ViewEncapsulation.None,
  template: `
    <app-resp-link-back-to-shop      [class]="'linkA '+customClass" ></app-resp-link-back-to-shop>
    <app-resp-link-orders-backlog    [class]="'linkB '+customClass" ></app-resp-link-orders-backlog>
    <app-resp-link-customer-accounts [class]="'linkC '+customClass" ></app-resp-link-customer-accounts>
    <app-resp-link-products          [class]="'linkD '+customClass" ></app-resp-link-products>
    <app-resp-link-bi                [class]="'linkE '+customClass" ></app-resp-link-bi>
    <app-resp-link-salehist          [class]="'linkF '+customClass" ></app-resp-link-salehist>
    <app-logout-link                 [class]="'linkG '+customClass" [isLoggedIn]="isLoggedIn" (logout)="logout.emit($event)" [showBar]="true"></app-logout-link>
  `
})
export class RespAdminLinksComponent {
  @Input() customClass: string = '';
  @Input() isLoggedIn: boolean = false;
  @Output() logout = new EventEmitter<MouseEvent>();
}
