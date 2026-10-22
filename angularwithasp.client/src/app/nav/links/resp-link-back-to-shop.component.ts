import { Component, Input, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';
import { VlComponent } from './vl.component';

@Component({
  selector: 'app-resp-link-back-to-shop',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent, VlComponent],
  encapsulation: ViewEncapsulation.None,
  template: `
    <div [class]="customClass">
      <app-responsive-link [toRoute]="toRoute" [tiny]="tiny" [small]="small" [regular]="regular"></app-responsive-link>@if(showBar){<app-vl></app-vl>}
    </div>

    <ng-template #tiny>Back to Shop</ng-template>
    <ng-template #small><span class="backToShopBtn">Back to Shop</span></ng-template>
    <ng-template #regular><span class="backToShopBtn"><i class="bi bi-arrow-left"></i>&nbsp;Back to Shop</span></ng-template>
  `
})
export class RespLinkBackToShopComponent {
  @Input() toRoute: string = '/';
  @Input() customClass: string = '';
  @Input() showBar: boolean = true;
}
