import { Component, Input, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';
import { VlComponent } from './vl.component';

@Component({
  selector: 'app-resp-link-customer-accounts',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent, VlComponent],
  encapsulation: ViewEncapsulation.None,
  template: `
    <div [class]="customClass">
      <app-vl></app-vl><app-responsive-link [toRoute]="toRoute" [tiny]="tiny" [small]="tiny" [regular]="regular"></app-responsive-link>
    </div>

    <ng-template #tiny>Customers</ng-template>
    <ng-template #regular>Customer Accounts</ng-template>
  `
})
export class RespLinkCustomerAccountsComponent {
  @Input() toRoute: string = '/admin/useraccounts';
  @Input() customClass: string = '';
}
