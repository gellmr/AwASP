import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';
import { VlComponent } from './vl.component';

@Component({
  selector: 'app-resp-link-admin-console',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent, VlComponent],
  template: `
    <div [class]="customClass">
      @if(showBar){<app-vl></app-vl>}<app-responsive-link [toRoute]="toRoute" [tiny]="compact" [small]="compact" [regular]="regular" [title]="tooltip"></app-responsive-link>
    </div>

    <ng-template #compact>Admin&nbsp;Console</ng-template>
    <ng-template #regular>Admin&nbsp;Console</ng-template>
  `
})
export class RespLinkAdminConsoleComponent {
  @Input() toRoute: string = '/admin/orders/1';
  @Input() customClass: string = '';
  @Input() showBar: boolean = true;
  @Input() tooltip: string = 'aaa';
}
