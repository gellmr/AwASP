import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-pagination-links',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './pagination-links.component.html',
  styleUrl: './pagination-links.component.css'
})
export class PaginationLinksComponent {
  @Input() numPages: number = 1;
  @Input() currPage: number = 1;
  @Input() myRoute: string = '/';

  get pages(): number[] {
    return Array.from({ length: this.numPages }, (_, i) => i + 1);
  }
}
