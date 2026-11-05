import { Component, inject, input, Signal, WritableSignal } from '@angular/core';
import { CartService, ProductEntry } from '../../../services/cart-service';
import { ProductEntryComponent } from '../product-entry/product-entry';

@Component({
  selector: 'app-order-list-component',
  imports: [ProductEntryComponent],
  templateUrl: './products-list-component.html',
  styleUrl: './products-list-component.css',
})
export class orderListComponent {
  array = input.required<WritableSignal<ProductEntry[]>>();
}
