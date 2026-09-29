import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { InStockProductCanAddComponent } from './in-stock-product-can-add/in-stock-product-can-add.component';
import { PaginationLinksComponent } from './pagination-links/pagination-links.component';
import { ProductSearchBoxComponent } from './product-search-box/product-search-box.component';
import { ProceedCartBtnComponent } from './proceed-cart-btn/proceed-cart-btn.component';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [CommonModule, InStockProductCanAddComponent, PaginationLinksComponent, ProductSearchBoxComponent, ProceedCartBtnComponent],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.css'
})
export class ShopComponent implements OnInit, OnDestroy {
  products: any[] = [];
  inStockProdThisPage: any[] = [];
  isLoading = true;
  error: any = null;
  
  searchTerm = '';
  numPages = 1;
  currPage = 1;
  myRoute = '/';
  gotItems = false;
  
  private currentCategory:       string | null = null;
  private lastFetchedCategory:   string | null = null;
  private lastFetchedSearchTerm: string = '';
  private gotItemsFromServer = false;

  // Use DI to get the services that we want
  private http        = inject(HttpClient);
  private route       = inject(ActivatedRoute);
  private router      = inject(Router);
  private cdr         = inject(ChangeDetectorRef);
  private cartService = inject(CartService);

  private cartSub = new Subscription();

  ngOnInit() {
    // Listen to changes in the route URL.
    this.route.paramMap.subscribe(params => {
      const category = params.get('category'); // detect if the category changed, in the route url.
      const page = params.get('page');         // detect if the page changed, in the route url.
      
      // Reset search filter if the user navigated to a different category
      if (category !== this.currentCategory) {
        this.searchTerm = '';
        this.currentCategory = category;
      }
      
      this.currPage = page ? parseInt(page, 10) : 1;
      this.myRoute = category ? `/category/${category}/` : '/';
      
      // Optimization: Avoid redundant HTTP calls if filters haven't changed
      if (category !== this.lastFetchedCategory || this.searchTerm !== this.lastFetchedSearchTerm || !this.gotItemsFromServer) {
        this.fetchProducts(category);
      } else {
        this.sliceProductsForPage();
        this.cdr.markForCheck(); // Manually notify change detector for zoneless rendering optimization
      }
    });

    // Subscribe to global cart state changes
    this.cartSub = this.cartService.cart$.subscribe(cartLines => {
      this.gotItems = cartLines.length > 0;
      this.cdr.markForCheck();
    });
  }

  ngOnDestroy() {
    // Clean up subscriptions to prevent memory leaks when leaving the component
    this.cartSub.unsubscribe();
  }

  handleSearchChange(term: string) {
    this.searchTerm = term;
    if (this.currPage !== 1) {
      // Reset back to page 1 on search change
      const category = this.route.snapshot.paramMap.get('category');
      if (category) {
        this.router.navigate(['/category', category]);
      } else {
        this.router.navigate(['/']);
      }
    } else {
      const category = this.route.snapshot.paramMap.get('category');
      this.fetchProducts(category);
    }
  }

  sliceProductsForPage() {
    const prodPerPage = 4;
    const maxWholePageNum = Math.floor(this.products.length / prodPerPage);
    const extraPage = (this.products.length % prodPerPage === 0) ? 0 : 1;
    this.numPages = maxWholePageNum + extraPage || 1;
    
    const pageIntP = (this.currPage > this.numPages) ? this.numPages : this.currPage;
    const pageIdx = pageIntP - 1;
    const startIdx = prodPerPage * pageIdx;
    const endIdx = startIdx + prodPerPage;
    
    // Extract subset of products for the active pagination page
    this.inStockProdThisPage = this.products.slice(startIdx, endIdx);
  }

  fetchProducts(category: string | null) {
    if (!this.gotItemsFromServer) {
      this.isLoading = true;
    }
    const basePath = category ? `/api/products/category/${category}` : '/api/products';
    const query = this.searchTerm ? `?search=${encodeURIComponent(this.searchTerm)}` : '';
    const url = `${basePath}${query}`;
    
    this.lastFetchedCategory = category;
    this.lastFetchedSearchTerm = this.searchTerm;
    this.gotItemsFromServer = true;

    // Fetch product list via HttpClient Observable
    this.http.get<any[]>(url).subscribe({
      next: (data) => {
        this.products = data;
        this.sliceProductsForPage();
        this.isLoading = false;
        this.cdr.markForCheck(); // Trigger view update under zoneless change detection
      },
      error: (err) => {
        this.error = err;
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
