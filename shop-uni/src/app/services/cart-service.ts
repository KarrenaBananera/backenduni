import { computed, Injectable, signal } from '@angular/core';
import { Product } from '../shared/models/product';

export class ProductEntry {
  quantity = signal(0);
  constructor(public product: Product) {}
}

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private readonly STORAGE_KEY = 'cart-data';

  private productsMap = new Map<number, ProductEntry>();

  productsCount = signal(0);

  itemsSignal = signal<ProductEntry[]>([]);
  
  subTotalPrice = computed ( () =>
  {
    let sum = 0;
    for (const product of this.itemsSignal())
    {
      sum += product.product.price * product.quantity();
    }
    return sum;
  });

  taxPrice = computed (() => Number((this.subTotalPrice() * 0.08).toFixed(2)) );

  discountPercent = signal(0);

  discountPrice = computed( () => Number((this.subTotalPrice()*this.discountPercent()).toFixed(2)));

  totalPrice = computed (()=>Number((
    this.subTotalPrice()
  + this.taxPrice() 
  - this.discountPrice())
  .toFixed(2)));

  constructor() {
    this.loadFromStorage();
  }

  private loadFromStorage() {
    const data = localStorage.getItem(this.STORAGE_KEY);
    if (!data) return;

    try {
      const parsed = JSON.parse(data);
      if (Array.isArray(parsed)) {
        this.productsMap = new Map(
          parsed.map(item => {
            const entry = new ProductEntry(item.product);
            entry.quantity.set(item.quantity);
            return [item.product.id, entry];
          })
        );
        this.updateCount();
      }
    } catch (e) {
    }
  }

  private saveToStorage() {
    const data = Array.from(this.productsMap.values()).map(entry => ({
      product: entry.product,
      quantity: entry.quantity()
    }));
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(data));
  }

  private updateCount() {
    let total = 0;
    for (const entry of this.productsMap.values()) {
      total += entry.quantity();
    }
    this.productsCount.set(total);

    this.itemsSignal.set(Array.from(this.productsMap.values()));

    this.saveToStorage();
  }

  addProduct(product: Product) {
    const id = product.id;
    const existing = this.productsMap.get(id);
    if (existing) {
      existing.quantity.set(existing.quantity() + 1);
    } else {
      const entry = new ProductEntry(product);
      entry.quantity.set(1);
      this.productsMap.set(id, entry);
    }
    this.updateCount();
  }

  addProducts(product: Product, count: number) {
    const id = product.id;
    const existing = this.productsMap.get(id);
    if (existing) {
      existing.quantity.set(existing.quantity() + count);
    } else {
      const entry = new ProductEntry(product);
      entry.quantity.set(count);
      this.productsMap.set(id, entry);
    }
    this.updateCount();
  }

  removeProduct(product: Product) {
    const id = product.id;
    const existing = this.productsMap.get(id);
    if (existing) {
      const newQty = existing.quantity() - 1;
      if (newQty <= 0) {
        this.productsMap.delete(id);
      } else {
        existing.quantity.set(newQty);
      }
      this.updateCount();
    }
  }

  clearProduct(product: Product) {
    this.productsMap.delete(product.id);
    this.updateCount();
  }
}