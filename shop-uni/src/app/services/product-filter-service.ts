import { computed, effect, inject, Injectable } from '@angular/core';
import { FilterSettings, SortSettings } from './model/filter-settings';
import { ProductService } from './product-service';

@Injectable({
  providedIn: 'root',
})
export class ProductFilterService {
  private productService = inject(ProductService);
  products = this.productService.products;
  public filter = new FilterSettings();
  public sort = new SortSettings();
  filteredCount = computed(() => this.productService.total());

  filteredProducts = computed(() => this.products());

  constructor() {
    effect(() => {
      const rating = this.filter.ratingFilter();
      const minPrice = this.filter.minPrice();
      const maxPrice = this.filter.maxPrice();
      const sort = this.sort.sortBy();
      const minRating = rating === '5plus' ? 5 : rating === '4plus' ? 4 : rating === '3plus' ? 3 : null;

      this.productService.load({
        minPrice,
        maxPrice,
        minRating,
        sortBy: sort === 'name' ? 'title' : 'price',
        order: sort === 'priceDesc' ? 'desc' : 'asc',
      });
    });
  }
}
