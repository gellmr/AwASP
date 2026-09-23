import { Component, Input, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-responsive-link',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <a [routerLink]="toRoute" routerLinkActive="active" [routerLinkActiveOptions]="toRoute === '/' ? { exact: true } : { exact: false }" class="useResponsiveLink xs d-inline-block d-sm-none" [ngStyle]="extraStyle">
      <ng-container *ngTemplateOutlet="tinyMarkup"></ng-container>
    </a>
    <a [routerLink]="toRoute" routerLinkActive="active" [routerLinkActiveOptions]="toRoute === '/' ? { exact: true } : { exact: false }" class="useResponsiveLink sm d-none d-sm-inline-block d-md-none" [ngStyle]="extraStyle">
      <ng-container *ngTemplateOutlet="smallMarkup"></ng-container>
    </a>
    <a [routerLink]="toRoute" routerLinkActive="active" [routerLinkActiveOptions]="toRoute === '/' ? { exact: true } : { exact: false }" class="useResponsiveLink md d-none d-sm-none d-md-inline-block" [ngStyle]="extraStyle">
      <ng-container *ngTemplateOutlet="markup"></ng-container>
    </a>
  `
})
export class ResponsiveLinkComponent {
  @Input() toRoute!: string;
  @Input() tinyMarkup!: TemplateRef<any>;
  @Input() smallMarkup!: TemplateRef<any>;
  @Input() markup!: TemplateRef<any>;
  @Input() extraStyle: any = { minWidth: '45px', textAlign: 'left' };
}

