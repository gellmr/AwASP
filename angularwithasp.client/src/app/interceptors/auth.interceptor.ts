
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { CartService } from '../services/cart.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const cartService = inject(CartService);
  const router = inject(Router);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401){
        console.info("Server says you are no longer logged in.")
        cartService.setUser(null); // Session has expired on the server
        router.navigate(['/admin/login']);
      }
      return throwError(() => error);
    })
  );
};
