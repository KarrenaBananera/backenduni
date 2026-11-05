import { Routes } from '@angular/router';
import { ProductsPageComponent } from './pages/products/products-page-component/products-page-component';
import { ProductDetailsPage } from './pages/product-details/product-details-page/product-details-page';
import { OrderPageComponent } from './pages/order-page/order-page-component/order-page-component';

export const routes: Routes = [
    {path: '', component: ProductsPageComponent},
    {path: 'cart', component: OrderPageComponent},
    { path: 'products/:id', component: ProductDetailsPage },
    { path: '**', redirectTo: '' },
];
