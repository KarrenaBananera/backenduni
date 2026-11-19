import { signal } from '@angular/core';

export type RatingFilter = 'all' | '5plus' | '4plus' | '3plus';
export type SortOption = 'name' | 'priceDesc' | 'priceAsc';

export class FilterSettings {
  ratingFilter = signal<RatingFilter>('all');
  minPrice = signal<number | null>(null);
  maxPrice = signal<number | null>(null);

  clearFilter()
  {
    this.ratingFilter.set('all');
    this.minPrice.set(null);
    this.maxPrice.set(null);
  }
}

export class SortSettings {
  sortBy = signal<SortOption>('name');
}