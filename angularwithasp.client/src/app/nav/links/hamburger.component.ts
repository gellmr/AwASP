import { Component, Input, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-hamburger',
  standalone: true,
  imports: [
    CommonModule
  ],
  encapsulation: ViewEncapsulation.None,
  template: `
    <div class="custom-hamburger-container">
      <div class="hamburger-top">
        <ng-content select="[sitename]"></ng-content>
        <ng-content select="[inlined]"></ng-content>
        <ng-content select="[pullright]"></ng-content>
        @if(showHamBurger){
          <i class="bi bi-list" (click)="toggleMenu()"></i>
        }
      </div>
      <div class="menu-dropdown" [class.show]="isMenuOpen"><ng-content select="[stacked]"></ng-content></div>
    </div>
  `,
  styles: `
    .custom-hamburger-container { display: flex; flex-direction: column; justify-content: top;}
    .hamburger-top              { display: flex; flex-direction: row;    justify-content: space-between; align-items: center;}
    div[sitename]               { display: flex; flex-direction: row;    justify-content: flex-start;    align-items: center; min-width: 130px;}
    div[pullright]              { display: flex; flex-direction: row;    justify-content: flex-end; width: 100%; max-height:50px;}
    div[inlined]                { display: flex; flex-direction: row;    justify-content: center;   width: 100%; max-height:50px;}
    #adminLayout div[inlined]   { justify-content: flex-end;}
    div[stacked]                { }
    .bi-list {
      border: 1px solid rgba(146, 146, 146, 0.4);
      color: gainsboro;
      padding: 3px;
      border-radius: 7px;
      font-size: 30px;
      cursor: pointer;
      display: flex;
      flex-direction: column;
      justify-content: center;
      height: 40px;
    }
    .menu-dropdown              { display: none;  }
    .menu-dropdown.show         { display: block; }
  `
})
export class HamburgerComponent {
  isMenuOpen = false;
  toggleMenu(): void { this.isMenuOpen = !this.isMenuOpen; }
  @Input() showHamBurger: boolean = true;
}
