import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';
import { VlComponent } from './vl.component';

@Component({
  selector: 'app-resp-link-shop',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent, VlComponent],
  template: `
    <div [class]="customClass">
      <app-responsive-link [toRoute]="toRoute" [tiny]="tiny" [small]="tiny" [regular]="tiny"></app-responsive-link><app-vl></app-vl>
    </div>

    <ng-template #tiny>Shop</ng-template>
  `
})
export class RespLinkShopComponent {
  @Input() toRoute: string = '/';
  @Input() customClass: string = '';
}
