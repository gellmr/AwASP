import { Component, Input, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';
import { VlComponent } from './vl.component';

@Component({
  selector: 'app-resp-link-products',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent, VlComponent],
  encapsulation: ViewEncapsulation.None,
  template: `
    <div [class]="customClass">
      <app-vl></app-vl><app-responsive-link [toRoute]="toRoute" [tiny]="tiny" [small]="tiny" [regular]="tiny"></app-responsive-link>
    </div>

    <ng-template #tiny>Products</ng-template>
  `
})
export class RespLinkProductsComponent {
  @Input() toRoute: string = '/admin/products';
  @Input() customClass: string = '';
}
