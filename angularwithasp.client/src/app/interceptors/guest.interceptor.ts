import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { CartService } from '../services/cart.service';

export const guestInterceptor: HttpInterceptorFn = (req, next) => {
  const cartService = inject(CartService);
  const guestId = cartService.getGuestId();
  if (guestId) {
    const clonedReq = req.clone({
      setHeaders: {
        'X-Guest-Id': guestId
      }
    });
    return next(clonedReq);
  }
  return next(req);
};
