import { Routes } from '@angular/router';
import { ProductsPageComponent } from './pages/products/products-page-component/products-page-component';
import { ProductDetailsPage } from './pages/product-details/product-details-page/product-details-page';
import { OrderPageComponent } from './pages/order-page/order-page-component/order-page-component';
import { AdminUsersPage } from './pages/admin-users/admin-users-page';
import { AdminPromoCodesPage } from './pages/admin-promo-codes/admin-promo-codes-page';
import { AdminProductPage } from './pages/admin-product/admin-product-page';
import { AccountPage } from './pages/account/account-page';
import { adminGuard } from './services/admin-guard';

export const routes: Routes = [
    {path: '', component: ProductsPageComponent},
    {path: 'cart', component: OrderPageComponent},
    { path: 'products/:id', component: ProductDetailsPage },
    { path: 'account', component: AccountPage },
    { path: 'admin/users', component: AdminUsersPage, canActivate: [adminGuard] },
    { path: 'admin/promo-codes', component: AdminPromoCodesPage, canActivate: [adminGuard] },
    { path: 'admin/products/new', component: AdminProductPage, canActivate: [adminGuard] },
    { path: '**', redirectTo: '' },
];
