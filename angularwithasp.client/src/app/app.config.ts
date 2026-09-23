import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, RouteReuseStrategy } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { guestInterceptor } from './interceptors/guest.interceptor';
import { CustomRouteReuseStrategy } from './route-reuse.strategy';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([guestInterceptor])),
    { provide: RouteReuseStrategy, useClass: CustomRouteReuseStrategy }
  ]
};
