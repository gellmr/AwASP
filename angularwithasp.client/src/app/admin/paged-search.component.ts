import { Directive, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Subscription, Subject, combineLatest, map } from 'rxjs';
import { debounceTime, distinctUntilChanged, startWith, switchMap, tap } from 'rxjs/operators';

@Directive()
export abstract class PagedSearchComponent implements OnInit, OnDestroy
{
  error: string | null  = null;
  isLoading: boolean    = false;

  totResults: number    = 0;  // eg 5000 records.
  rowsPerPage: number   = 10; // Number of rows to show on each page of results.
  totPages: number      = 0;  // eg 500 pages == 5000 results / 10 rows per page.
  currPage: number      = 1;
  
  currWindow: number    = 1;
  searchTerm: string    = "";

  protected http          = inject(HttpClient);
  protected router        = inject(Router);
  protected route         = inject(ActivatedRoute);
  protected cdr           = inject(ChangeDetectorRef);
  protected sub           = new Subscription();
  
  protected searchSubject = new Subject<string>(); // stream of search inputs

  // Abstract members that child classes must implement
  abstract getRoutePrefix(): string[];
  abstract fetchData(page: number, search: string): import('rxjs').Observable<any>;
  abstract processResponse(data: any): void;

  ngOnInit() {
    this.sub.add(
      combineLatest([
        this.route.paramMap.pipe(map(params => params.get('page'))),
        this.searchSubject.pipe(
          debounceTime(300),
          distinctUntilChanged(),
          startWith('')
        )
      ]).pipe(
        tap(() => {
          this.isLoading = true;
          this.error = null;
          this.cdr.markForCheck();
        }),
        switchMap(([pageStr, searchTerm]) => {
          this.currPage = pageStr ? parseInt(pageStr, 10) : 1;
          this.searchTerm = searchTerm;
          return this.fetchData(this.currPage, this.searchTerm);
        })
      ).subscribe({
        next: (data) => {
          this.processResponse(data);
          this.isLoading = false;
          this.cdr.markForCheck();
        },
        error: (err) => {
          this.isLoading = false;
          if (err.status === 401) {
            console.log("Admin not logged in. Redirect to login...");
            this.router.navigate(['/admin']);
          } else {
            this.error = err.error?.errMessage || "Something went wrong";
          }
          this.cdr.markForCheck();
        }
      })
    );
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  handleSearchChange(str: string) {
    if (this.currPage !== 1) {
      this.router.navigate([...this.getRoutePrefix(), 1]);
    }
    this.searchSubject.next(str);
  }
}
