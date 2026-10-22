import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
  selector: 'app-pag-links',
  standalone: true,
  imports: [CommonModule, RouterModule, MatPaginatorModule],
  templateUrl: './pag-links.component.html',
  styleUrl:    './pag-links.component.css'
})
export class PagLinksComponent
{
  @Input() totPages: number = 1;
  @Input() length: number   = 1;
  @Input() currPage: number = 1;
  @Input() myRoute: string  = '/';

  public windowSize = 10;
  private router    = inject(Router);
  
  onPageChange(event: PageEvent) {
    this.currPage = event.pageIndex + 1; // mat paginator gives zero based page index. Routes uses 1-based index.
    this.windowSize = event.pageSize;
    this.router.navigate([this.myRoute, this.currPage]); // Navigate to target page via the router.
  }
  
  //   0  1  2  3  4  5  6  7  8  9     10 11 12 13 14 15   zero based page index
  //   1  2  3  4  5  6  7  8  9  10    11 12 13 14 15 16   page number
  //   0  0  0  0  0  0  0  0  0  0     1  1  1  1  1  1    window index (zero based)
  //   1  1  1  1  1  1  1  1  1  1     2  2  2  2  2  2    window index
  //   ^
}
