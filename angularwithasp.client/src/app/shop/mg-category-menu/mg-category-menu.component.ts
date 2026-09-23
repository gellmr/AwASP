import { Component, Input, HostBinding } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-mg-category-menu',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './mg-category-menu.component.html',
  styleUrl: './mg-category-menu.component.css'
})
export class MgCategoryMenuComponent {
  @HostBinding('class') class = 'w-100';
  @Input() isVertical: boolean = false;
}
