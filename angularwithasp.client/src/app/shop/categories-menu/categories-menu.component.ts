import { Component, OnInit, inject, HostBinding } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { MgCategoryMenuComponent } from '../mg-category-menu/mg-category-menu.component';
import { CatCaretComponent } from '../cat-caret/cat-caret.component';

@Component({
  selector: 'app-categories-menu',
  standalone: true,
  imports: [CommonModule, RouterModule, MgCategoryMenuComponent, CatCaretComponent],
  templateUrl: './categories-menu.component.html',
  styleUrl: './categories-menu.component.css'
})
export class CategoriesMenuComponent implements OnInit {

  @HostBinding('class') class = 'col-12 col-md-3 d-md-block';

  categories: any[] = [];
  isLoading = true;
  error: any = null;

  private http = inject(HttpClient);

  ngOnInit() {
    this.fetchCategories();
  }

  fetchCategories() {
    this.http.get<any[]>('/api/categories').subscribe({
      next: (data) => {
        this.categories = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.error = err;
        this.isLoading = false;
      }
    });
  }
}
