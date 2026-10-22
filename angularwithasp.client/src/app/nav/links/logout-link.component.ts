import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ResponsiveLinkComponent } from './responsive-link.component';
import { VlComponent } from './vl.component';

@Component({
  selector: 'app-logout-link',
  standalone: true,
  imports: [
    ResponsiveLinkComponent,
    VlComponent,
    CommonModule
  ],
  template: `
    @if (isLoggedIn) {
      <div [class]="customClass" (click)="onLogout($event)" style="cursor: pointer;">
        @if(showBar){<app-vl></app-vl>}<app-responsive-link [tiny]="compact" [small]="compact" [regular]="regular" [title]="tooltip"></app-responsive-link>
      </div>
    }
    <ng-template #compact>Logout</ng-template>
    <ng-template #regular>Logout Admin</ng-template>
  `
})
export class LogoutLinkComponent
{
  @Input() isLoggedIn: boolean = false;
  @Input() customClass: string = '';
  
  @Input() showBar: boolean = false;
  @Input() tooltip: string  = '(Logout) Admin Console';
  @Output() logout = new EventEmitter<MouseEvent>();

  onLogout(event: MouseEvent) {
    event.preventDefault(); // Prevent default anchor navigation
    this.logout.emit(event);
  }
}
