import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';

// Dedicated link components
import { RespLinkShopComponent }         from './resp-link-shop.component';
import { RespLinkCartComponent }         from './resp-link-cart.component';
import { RespLinkOrdersComponent }       from './resp-link-orders.component';
import { RespLinkAdminConsoleComponent } from './resp-link-admin-console.component';
import { LogoutLinkComponent } from './logout-link.component';

@Component({
  selector: 'app-resp-shoplinks',
  standalone: true,
  imports: [
    RespLinkShopComponent,
    RespLinkCartComponent,
    RespLinkOrdersComponent,
    RespLinkAdminConsoleComponent,
    LogoutLinkComponent,
    CommonModule
  ],
  encapsulation: ViewEncapsulation.None,
  template: `
    <app-resp-link-shop          [class]="'linkA '+customClass" ></app-resp-link-shop>
    <app-resp-link-cart          [class]="'linkB '+customClass" [cartQty]="cartQty"></app-resp-link-cart>
    <app-resp-link-orders        [class]="'linkC '+customClass" [ordersQty]="ordersQty"></app-resp-link-orders>
    <app-resp-link-admin-console [class]="'linkD '+customClass" [showBar]=true tooltip='Go to console' *ngIf="isLoggedIn" ></app-resp-link-admin-console>
    <app-resp-link-admin-console [class]="'linkD '+customClass" [showBar]=true tooltip='Login - Admin Console' *ngIf="!isLoggedIn" ></app-resp-link-admin-console>
    <app-logout-link             [class]="'linkE '+customClass" [isLoggedIn]="isLoggedIn" (logout)="logout.emit($event)" [showBar]="true"></app-logout-link>
  `
})
export class RespShopLinksComponent
{
  @Input() customClass: string = '';

  @Input() cartQty:   number = 0;
  @Input() ordersQty: number = 0;
  @Input() isLoggedIn: boolean = false;
  
  @Output() logout = new EventEmitter<MouseEvent>();
}
