import { Component, Input, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';
import { VlComponent } from './vl.component';

@Component({
  selector: 'app-resp-link-bi',
  standalone: true,
  imports: [CommonModule, ResponsiveLinkComponent, VlComponent],
  encapsulation: ViewEncapsulation.None,
  template: `
    <div [class]="customClass">
      <app-vl></app-vl><app-responsive-link [toRoute]="toRoute" [tiny]="tiny" [small]="tiny" [regular]="tiny"></app-responsive-link>
    </div>

    <ng-template #tiny>
      <span class="bi-center">BI</span>
    </ng-template>
  `
})
export class RespLinkBiComponent {
  @Input() toRoute: string = '/admin/productsales';
  @Input() customClass: string = '';
}
