import { Component, Input, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';

@Component({
  selector: 'app-resp-link-orders-backlog',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent],
  encapsulation: ViewEncapsulation.None,
  template: `
    <div [class]="customClass">
      <app-responsive-link [toRoute]="toRoute" [tiny]="tiny" [small]="tiny" [regular]="regular"></app-responsive-link>
    </div>

    <ng-template #tiny>Backlog</ng-template>
    <ng-template #regular>Orders Backlog</ng-template>
  `
})
export class RespLinkOrdersBacklogComponent {
  @Input() toRoute: string = '/admin/orders/1';
  @Input() customClass: string = '';
}
