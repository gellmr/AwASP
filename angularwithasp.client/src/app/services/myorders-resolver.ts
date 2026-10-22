import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { CartService } from './cart.service';
import { catchError } from 'rxjs/operators'; // or directly from 'rxjs' in newer versions
import { of } from 'rxjs';

export const myOrdersResolver: ResolveFn<any> = (route, state) => {
  const cartService = inject(CartService);
  return cartService.fetchMyOrders().pipe(
    catchError((error) => {
      // Return a safe fallback or handle the error
      return of({ rows: [] });
    })
  );
};
