import { Routes } from '@angular/router';
import { ShopLayoutComponent } from './layouts/shop-layout/shop-layout.component';
import { ShopComponent } from './shop/shop.component';
import { CartComponent } from './shop/cart/cart.component';
import { CheckoutComponent } from './shop/checkout/checkout.component';
import { CheckoutSuccessComponent } from './shop/checkout-success/checkout-success.component';
import { MyOrdersComponent } from './shop/my-orders/my-orders.component';
import { MyOrderDetailComponent } from './shop/my-order-detail/my-order-detail.component';
import { LoginLayoutComponent } from './layouts/login-layout/login-layout.component';
import { AdminLoginComponent } from './admin/admin-login/admin-login.component';
import { AdminLayoutComponent } from './admin/admin-layout/admin-layout.component';
import { AdminOrdersComponent } from './admin/admin-orders/admin-orders.component';
import { AdminOrderComponent } from './admin/admin-order/admin-order.component';
import { AdminProductsComponent } from './admin/admin-products/admin-products.component';
import { AdminUserAccountsComponent } from './admin/admin-user-accounts/admin-user-accounts.component';
import { AdminUserEditComponent } from './admin/admin-user-edit/admin-user-edit.component';
import { AdminUserOrdersComponent } from './admin/admin-user-orders/admin-user-orders.component';

export const routes: Routes = [
    {
        path: 'admin/orders',
        component: AdminLayoutComponent,
        children: [
            { path: '', component: AdminOrdersComponent },
            { path: ':page', component: AdminOrdersComponent }
        ]
    },
    {
        path: 'admin/order',
        component: AdminLayoutComponent,
        children: [
            { path: ':id', component: AdminOrderComponent }
        ]
    },
    {
        path: 'admin/products',
        component: AdminLayoutComponent,
        children: [
            { path: '', component: AdminProductsComponent }
        ]
    },
    {
        path: 'admin/useraccounts',
        component: AdminLayoutComponent,
        children: [
            { path: '', component: AdminUserAccountsComponent }
        ]
    },
    {
        path: 'admin/user',
        component: AdminLayoutComponent,
        children: [
            { path: ':id/edit', component: AdminUserEditComponent },
            { path: ':id/orders', component: AdminUserOrdersComponent }
        ]
    },
    {
        path: 'admin/guest',
        component: AdminLayoutComponent,
        children: [
            { path: ':id/edit', component: AdminUserEditComponent },
            { path: ':id/orders', component: AdminUserOrdersComponent }
        ]
    },
    {
        path: 'admin',
        component: LoginLayoutComponent,
        children: [
            { path: '', component: AdminLoginComponent }
        ]
    },
    { 
        path: '', 
        component: ShopLayoutComponent,
        children: [
            { path: '', component: ShopComponent },
            { path: 'cart', component: CartComponent },
            { path: 'checkout', component: CheckoutComponent },
            { path: 'checkoutsuccess', component: CheckoutSuccessComponent },
            { path: 'myorders', component: MyOrdersComponent },
            { path: 'myorders/:page', component: MyOrdersComponent },
            { path: 'myorder/:id', component: MyOrderDetailComponent },
            { path: ':page', component: ShopComponent },
            { path: 'category/:category', component: ShopComponent },
            { path: 'category/:category/:page', component: ShopComponent }
        ]
    },
    { path: '**', redirectTo: '' }
];

