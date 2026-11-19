import { Component, inject } from '@angular/core';
import { NgxSliderModule, Options } from '@angular-slider/ngx-slider';
import { FilterSettings, RatingFilter } from '../../../services/model/filter-settings';
import { ProductFilterService } from '../../../services/product-filter-service';

@Component({
  selector: 'app-filter-component',
  imports: [NgxSliderModule], 
  templateUrl: './filter-component.html',
  styleUrls: ['./filter-component.css'] 
})
export class FilterComponent {

  filterSettings = inject(ProductFilterService); 

  sliderOptions: Options = {
  floor: 0,
  ceil: 500,
  step: 10,
  animate: false,
  showSelectionBar: true,
  hidePointerLabels: true,
  hideLimitLabels: true
};

  selectRating(value: RatingFilter) {
    if (value === this.filterSettings.filter.ratingFilter())
      this.filterSettings.filter.ratingFilter.set('all');
    else
      this.filterSettings.filter.ratingFilter.set(value);
  }

   get sliderMaxPrice(): number {
    return this.filterSettings.filter.maxPrice() ?? 500;
  }
  set sliderMaxPrice(value: number) {
    this.filterSettings.filter.maxPrice.set(value);
  }

  get sliderMinPrice(): number {
    return this.filterSettings.filter.minPrice() ?? 0;
  }
  set sliderMinPrice(value: number) {
    this.filterSettings.filter.minPrice.set(value);
  }

  clearFilters()
  {
    this.filterSettings.filter.maxPrice.set(null);
    this.filterSettings.filter.minPrice.set(null);
    this.filterSettings.filter.ratingFilter.set('all');
    

  }
}