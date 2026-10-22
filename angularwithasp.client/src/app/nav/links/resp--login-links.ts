import { Component, Input, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';

// Dedicated link components
import { RespLinkBackToShopComponent }   from './resp-link-back-to-shop.component';

@Component({
  selector: 'app-resp-loginlinks',
  standalone: true,
  imports: [
    RespLinkBackToShopComponent,
    CommonModule
  ],
  encapsulation: ViewEncapsulation.None,
  template: `
    <app-resp-link-back-to-shop  [class]="'linkB '+customClass" [showBar]="false"></app-resp-link-back-to-shop>
  `
})
export class RespLoginLinksComponent{
  @Input() customClass: string = ''; 
}
