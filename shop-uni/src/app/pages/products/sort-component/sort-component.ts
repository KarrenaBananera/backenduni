import { Component, inject } from '@angular/core';
import { ProductFilterService } from '../../../services/product-filter-service';

@Component({
  selector: 'app-sort-component',
  imports: [],
  templateUrl: './sort-component.html',
  styleUrl: './sort-component.css',
})
export class SortComponent {
  filter = inject(ProductFilterService).sort.sortBy;

  onSelectChange(event: Event){
    const value = (event.target as HTMLSelectElement).value;
   switch(value)
   {
    case "nameAsc": this.filter.set("name"); break;
    case "priceAsc": this.filter.set("priceAsc"); break;
    case "priceDesc": this.filter.set("priceDesc"); break;
   }
  }
}
