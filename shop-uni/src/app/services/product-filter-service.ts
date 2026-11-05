import { computed, inject, Injectable, Signal } from '@angular/core';
import { FilterSettings, SortSettings } from './model/filter-settings';
import { ProductService } from './product-service';
import { Product } from '../shared/models/product';


@Injectable({
  providedIn: 'root'
})
export class ProductFilterService {
  private productService = inject(ProductService);
  products = this.productService.products;
  public filter = new FilterSettings();
  public sort = new SortSettings();
  filteredCount = computed(() => this.filteredProducts().length);

  filteredProducts = computed(() => {
    const allProducts = this.products();
    const { ratingFilter, minPrice, maxPrice } = this.filter;
    const { sortBy } = this.sort;

    let filtered = allProducts.filter(product => {
      if (ratingFilter() === '5plus' && product.rating < 5) return false;
      if (ratingFilter() === '4plus' && product.rating < 4) return false;
      if (ratingFilter() === '3plus' && product.rating < 3) return false;
      if (minPrice() !== null && product.price < minPrice()!) return false;
      if (maxPrice() !== null && product.price > maxPrice()!) return false;
      return true;
    });

    const sort = sortBy();
    switch (sort) {
      case 'name':
        filtered.sort((a, b) => a.name.localeCompare(b.name));
        break;
      case 'priceDesc':
        filtered.sort((a, b) => b.price - a.price);
        break;
      case 'priceAsc':
        filtered.sort((a, b) => a.price - b.price);
        break;
    }
    return filtered;
  });

}