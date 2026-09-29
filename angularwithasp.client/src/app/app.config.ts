import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, RouteReuseStrategy } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { guestInterceptor } from './interceptors/guest.interceptor';
import { CustomRouteReuseStrategy } from './route-reuse.strategy';

export const appConfig: ApplicationConfig = {
  providers: [

    // Catches unhandled runtime script errors and unhandled promise rejections that happen anywhere in the browser.
    // Forwards them to Angular's centralized ErrorHandler service.
    provideBrowserGlobalErrorListeners(),

    // Register our routes array with the Angular DI system.
    provideRouter(routes),

    // Register the Angular http client, so we can make API requests.
    // Wires the guestInterceptor to inspect/modify outgoing http requests.
    provideHttpClient(withInterceptors([guestInterceptor])),

    // Dont destroy + recreate route components every time we navigate.
    { provide: RouteReuseStrategy, useClass: CustomRouteReuseStrategy }
  ]
};
