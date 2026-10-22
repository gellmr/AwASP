import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';

@Component({
  selector: 'app-resp-link-cart',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent],
  template: `
    <div [class]="customClass">
      <app-responsive-link [toRoute]="toRoute" [tiny]="tiny" [small]="tiny" [regular]="tiny"></app-responsive-link>
    </div>

    <ng-template #tiny>
      <div style="display:inline-block;">
        <i class="bi bi-cart3" style="margin-right:4px;display:inline-block;"></i>Cart:&nbsp;{{cartQty}}<span class="hideInlineBlockXXs">&nbsp;Items</span>
      </div>
    </ng-template>
  `
})
export class RespLinkCartComponent {
  @Input() toRoute: string = '/cart';
  @Input() customClass: string = '';
  @Input() cartQty: number = 0;
}
