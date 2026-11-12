import { HttpClient } from '@angular/common/http';
import { computed, effect, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from '../shared/models/product';
import { APP_CONFIG } from '../app-config';
import { AuthService } from './auth-service';

export class ProductEntry {
  quantity = signal(0);
  cartItemId = '';
  constructor(public product: Product) {}
}

interface CartItemDto {
  id: string;
  productId: number;
  title: string;
  imageUrl: string | null;
  price: number;
  quantity: number;
}

interface CartDto {
  items: CartItemDto[];
  subtotal: number;
  discountPercentage: number;
  discountAmount: number;
  total: number;
}

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private http = inject(HttpClient);
  private apiUrl = inject(APP_CONFIG).apiUrl.replace(/\/$/, '');
  private auth = inject(AuthService);
  private router = inject(Router);

  productsCount = signal(0);
  itemsSignal = signal<ProductEntry[]>([]);
  subTotalPrice = signal(0);
  discountPercent = signal(0);
  discountPrice = signal(0);
  taxPrice = computed(() => Number((this.subTotalPrice() * 0.08).toFixed(2)));
  totalPrice = computed(() =>
    Number((this.subTotalPrice() + this.taxPrice() - this.discountPrice()).toFixed(2)),
  );

  constructor() {
    effect(() => {
      if (this.auth.token()) {
        this.refresh();
      } else {
        this.clearState();
      }
    });
  }

  addProduct(product: Product) {
    this.addProducts(product, 1);
  }

  addProducts(product: Product, count: number) {
    if (!this.ensureSignedIn()) {
      return;
    }

    this.http
      .post<CartDto>(`${this.apiUrl}/cart/items`, { productId: product.id, quantity: count })
      .subscribe((cart) => this.apply(cart));
  }

  removeProduct(product: Product) {
    const entry = this.itemsSignal().find((item) => item.product.id === product.id);
    if (!entry || !this.ensureSignedIn()) {
      return;
    }

    if (entry.quantity() <= 1) {
      this.clearProduct(product);
      return;
    }

    this.http
      .patch<CartDto>(`${this.apiUrl}/cart/items/${entry.cartItemId}`, {
        quantity: entry.quantity() - 1,
      })
      .subscribe((cart) => this.apply(cart));
  }

  clearProduct(product: Product) {
    const entry = this.itemsSignal().find((item) => item.product.id === product.id);
    if (!entry || !this.ensureSignedIn()) {
      return;
    }

    this.http
      .delete<CartDto>(`${this.apiUrl}/cart/items/${entry.cartItemId}`)
      .subscribe((cart) => this.apply(cart));
  }

  applyPromo(code: string) {
    if (!this.ensureSignedIn()) {
      return;
    }

    this.http
      .post<CartDto>(`${this.apiUrl}/cart/apply-promo`, { code })
      .subscribe((cart) => this.apply(cart));
  }

  private refresh() {
    this.http.get<CartDto>(`${this.apiUrl}/cart`).subscribe((cart) => this.apply(cart));
  }

  private apply(cart: CartDto) {
    const entries = cart.items.map((item) => {
      const image = item.imageUrl ?? '';
      const product = new Product(
        item.productId,
        item.title,
        image,
        image ? [image] : [],
        {},
        item.price,
        0,
        '',
        '',
      );
      const entry = new ProductEntry(product);
      entry.quantity.set(item.quantity);
      entry.cartItemId = item.id;
      return entry;
    });

    this.itemsSignal.set(entries);
    this.productsCount.set(entries.reduce((sum, entry) => sum + entry.quantity(), 0));
    this.subTotalPrice.set(cart.subtotal);
    this.discountPercent.set(cart.discountPercentage / 100);
    this.discountPrice.set(cart.discountAmount);
  }

  private clearState() {
    this.itemsSignal.set([]);
    this.productsCount.set(0);
    this.subTotalPrice.set(0);
    this.discountPercent.set(0);
    this.discountPrice.set(0);
  }

  private ensureSignedIn() {
    if (this.auth.isSignedIn()) {
      return true;
    }

    this.router.navigateByUrl('/account');
    return false;
  }
}
