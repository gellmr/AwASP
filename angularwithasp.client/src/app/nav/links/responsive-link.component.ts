import { Component, Input, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-responsive-link',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    @if (toRoute === null)
    {
      <a class="useResponsiveLink xs d-inline-block d-sm-none" [ngStyle]="extraStyle">
        <ng-container *ngTemplateOutlet="tiny"></ng-container>
      </a>
      <a class="useResponsiveLink sm d-none d-sm-inline-block d-lg-none" [ngStyle]="extraStyle">
        <ng-container *ngTemplateOutlet="small"></ng-container>
      </a>
      <a class="useResponsiveLink md d-none d-lg-inline-block" [ngStyle]="extraStyle">
        <ng-container *ngTemplateOutlet="regular"></ng-container>
      </a>
    }
    @else
    {
      <a [routerLink]="toRoute" routerLinkActive="active" [routerLinkActiveOptions]="toRoute === '/' ? { exact: true } : { exact: false }" class="useResponsiveLink xs d-inline-block d-sm-none" [ngStyle]="extraStyle">
        <ng-container *ngTemplateOutlet="tiny"></ng-container>
      </a>
      <a [routerLink]="toRoute" routerLinkActive="active" [routerLinkActiveOptions]="toRoute === '/' ? { exact: true } : { exact: false }" class="useResponsiveLink sm d-none d-sm-inline-block d-lg-none" [ngStyle]="extraStyle">
        <ng-container *ngTemplateOutlet="small"></ng-container>
      </a>
      <a [routerLink]="toRoute" routerLinkActive="active" [routerLinkActiveOptions]="toRoute === '/' ? { exact: true } : { exact: false }" class="useResponsiveLink md d-none d-lg-inline-block" [ngStyle]="extraStyle">
        <ng-container *ngTemplateOutlet="regular"></ng-container>
      </a>
    }
  `
})
export class ResponsiveLinkComponent
{
  @Input() toRoute: string | null = null;
  
  @Input() tiny?: TemplateRef<any>;
  @Input() small?: TemplateRef<any>;
  @Input() regular?: TemplateRef<any>;

  @Input() extraStyle: any = { minWidth: '45px', textAlign: 'left' };
}

