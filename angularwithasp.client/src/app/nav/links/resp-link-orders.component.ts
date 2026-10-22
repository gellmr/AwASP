import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';
import { VlComponent } from './vl.component';

@Component({
  selector: 'app-resp-link-orders',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent, VlComponent],
  template: `
    <div [class]="customClass">
      <app-vl></app-vl><app-responsive-link [toRoute]="toRoute" [tiny]="compact" [small]="compact" [regular]="regular"></app-responsive-link>
    </div>

    <ng-template #compact>Orders<span *ngIf="ordersQty > 0">&nbsp;({{ordersQty}})</span></ng-template>
    <ng-template #regular>My&nbsp;Orders<span *ngIf="ordersQty > 0">&nbsp;({{ordersQty}})</span></ng-template>
  `
})
export class RespLinkOrdersComponent {
  @Input() toRoute: string = '/myorders';
  @Input() customClass: string = '';
  @Input() ordersQty: number = 0;
}
