import { RouteReuseStrategy, ActivatedRouteSnapshot, DetachedRouteHandle } from '@angular/router';

export class CustomRouteReuseStrategy implements RouteReuseStrategy
{
  // Use default behavior
  shouldDetach(route: ActivatedRouteSnapshot): boolean { return false; }
  store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle | null): void {}
  shouldAttach(route: ActivatedRouteSnapshot): boolean { return false; }
  retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null { return null; }

  // Custom behavior - avoid expensive destroy and recreate of route components
  shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean
  {
    // Check if the incoming route (future) and current route (curr)
    // share the exact same route configuration definition from your routes file.
    // If they do (like navigating from `/users/1` to `/users/2` on the same route path rule),
    // then reuse the component immediately (dont destroy and recreate it)
    if (future.routeConfig === curr.routeConfig) {
      return true;
    }

    // Fallback check: If they don't share the same route definition, verify if both routes point 
    // to the exact same underlying component class (and ensure it's not null/undefined).
    // If two different route paths load the same component, this reuses it.
    // (Avoid expensive teardown and rebuild)
    return future.component === curr.component && future.component !== null;
  }
}
