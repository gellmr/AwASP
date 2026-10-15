import { Routes } from '@angular/router';

import { LoginLayoutComponent } from './layouts/login-layout/login-layout.component';
import { AdminLoginComponent }  from './admin/admin-login/admin-login.component';

import { AdminLayoutComponent }       from './admin/admin-layout/admin-layout.component';
import { AdminOrderComponent }        from './admin/admin-order/admin-order.component';
import { AdminOrdersComponent }       from './admin/admin-orders/admin-orders.component';
import { AdminUserEditComponent }     from './admin/admin-user-edit/admin-user-edit.component';
import { AdminUserOrdersComponent }   from './admin/admin-user-orders/admin-user-orders.component';
import { AdminProductsComponent }     from './admin/admin-products/admin-products.component';
import { AdminUserAccountsComponent } from './admin/admin-user-accounts/admin-user-accounts.component';

import { ShopLayoutComponent }      from './layouts/shop-layout/shop-layout.component';
import { CheckoutSuccessComponent } from './shop/checkout-success/checkout-success.component';
import { CheckoutComponent }        from './shop/checkout/checkout.component';
import { MyOrderDetailComponent }   from './shop/my-order-detail/my-order-detail.component';
import { MyOrdersComponent }        from './shop/my-orders/my-orders.component';
import { CartComponent }            from './shop/cart/cart.component';
import { ShopComponent }            from './shop/shop.component';

export const routes: Routes = [

    // admin login route
    {
        path: 'admin',
        component: LoginLayoutComponent,
        children: [
            { path: '', component: AdminLoginComponent }
        ]
    },

    // protected admin routes
    {
        path: 'admin',
        component: AdminLayoutComponent,
        children: [
            { path: 'order/:id',     component: AdminOrderComponent },
            { path: 'orders/:page',  component: AdminOrdersComponent },
            { path: 'orders',        component: AdminOrdersComponent },
            { path: 'user/:id/edit',     component: AdminUserEditComponent },
            { path: 'user/:id/orders',   component: AdminUserOrdersComponent },
            { path: 'guest/:id/edit',       component: AdminUserEditComponent },
            { path: 'guest/:id/orders',     component: AdminUserOrdersComponent },
            { path: 'products',       component: AdminProductsComponent },
            { path: 'useraccounts',   component: AdminUserAccountsComponent },
        ]
    },
    
    // shop routes
    { 
        path: '', 
        component: ShopLayoutComponent,
        children: [
            { path: 'checkoutsuccess',     component: CheckoutSuccessComponent },
            { path: 'checkout',            component: CheckoutComponent },
            { path: 'myorder/:id',      component: MyOrderDetailComponent },
            { path: 'myorders/:page',   component: MyOrdersComponent },
            { path: 'myorders',         component: MyOrdersComponent },
            { path: 'cart',             component: CartComponent },
            { path: 'category/:category/:page', component: ShopComponent },
            { path: 'category/:category',       component: ShopComponent },
            { path: ':page',                    component: ShopComponent },
            { path: '',                         component: ShopComponent },
        ]
    },

    // default
    { path: '**', redirectTo: '' }
];

