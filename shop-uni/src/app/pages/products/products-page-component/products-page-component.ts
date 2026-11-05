import { Component, inject } from '@angular/core';
import { FilterComponent } from "../filter-component/filter-component";
import { ProductListComponent } from "../product-list-component/product-list-component";
import { CartService } from '../../../services/cart-service';
import { Product } from '../../../shared/models/product';
import { ToastrService } from 'ngx-toastr';
import { ProductFilterService } from '../../../services/product-filter-service';
import { SortComponent } from "../sort-component/sort-component";
import { Router } from '@angular/router';

@Component({
  selector: 'app-products-page-component',
  imports: [FilterComponent, ProductListComponent, SortComponent],
  templateUrl: './products-page-component.html',
  styleUrl: './products-page-component.css',
})
export class ProductsPageComponent {
  cart = inject(CartService);
  router = inject(Router);
  toastr = inject(ToastrService);
  filter = inject(ProductFilterService);
  showFilter = false;

  onAddProductToCart(product: Product)
  {
    this.cart.addProduct(product);
    this.toastr.success("",`Added ${product.name} to cart`);
  }
  
  onFilterClick()
  {
    this.showFilter = !this.showFilter;
  }

  onProductClick(product: Product)
  {
    this.router.navigate(['/products', product.id]);
  }
}
