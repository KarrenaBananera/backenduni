import { Component, computed, inject, input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { NgOptimizedImage } from '@angular/common';
import { CartService, ProductEntry } from '../../../services/cart-service';

@Component({
  selector: 'app-product-entry',
  imports: [NgOptimizedImage],
  templateUrl: './product-entry.html',
  styleUrl: './product-entry.css',
})
export class ProductEntryComponent {
  productEntry = input.required<ProductEntry>();
  product : Product | undefined = undefined;
  cart = inject(CartService);

  quantity = computed(() => this.cart.itemsSignal)

  ngOnInit()
  {
    this.product = this.productEntry().product;
  }

  onMinusClick()
  {
    this.cart.removeProduct(this.product!);
  }

  onPlusClick()
  {
    this.cart.addProduct(this.product!);
  }

  onGarbageClick()
  {
    this.cart.clearProduct(this.product!);
  }
}
