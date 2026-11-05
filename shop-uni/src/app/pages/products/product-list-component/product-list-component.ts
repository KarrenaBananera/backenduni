import { CommonModule } from '@angular/common';
import { Component, computed, inject, output, Signal } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { ProductService } from '../../../services/product-service';
import { CardComponent } from '../../../shared/components/card-component/card-component';
import { ProductFilterService } from '../../../services/product-filter-service';

@Component({
  selector: 'app-product-list-component',
  imports: [CommonModule, CardComponent],
  templateUrl: './product-list-component.html',
  styleUrl: './product-list-component.css',
})
export class ProductListComponent {
  filterService = inject(ProductFilterService);

  addToCartEvent = output<Product>();
  productClickEvent = output<Product>();

  onComponentClick(product: Product) {
    this.productClickEvent.emit(product);
  }

  OnAddToCartClick(product: Product) {
    this.addToCartEvent.emit(product);
  }

  clearFilter()
  {
    this.filterService.filter.clearFilter();
  }
}
