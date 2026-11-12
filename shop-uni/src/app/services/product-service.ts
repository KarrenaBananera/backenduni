import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import { APP_CONFIG } from '../app-config';
import { Product } from '../shared/models/product';

interface ProductDto {
  id: number;
  title: string;
  description: string;
  price: number;
  rating: number;
  images: string[];
  characteristics: Record<string, string>;
}

interface PagedProductsResponse {
  items: ProductDto[];
  meta: { total_items: number };
}

export interface ProductQuery {
  minPrice: number | null;
  maxPrice: number | null;
  minRating: number | null;
  sortBy: 'title' | 'price';
  order: 'asc' | 'desc';
}

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private http = inject(HttpClient);
  private apiUrl = inject(APP_CONFIG).apiUrl.replace(/\/$/, '');

  readonly products = signal<Product[]>([]);
  readonly total = signal(0);

  load(query: ProductQuery) {
    let params = new HttpParams().set('page', '1').set('limit', '100');
    if (query.minPrice !== null) {
      params = params.set('min_price', query.minPrice);
    }
    if (query.maxPrice !== null) {
      params = params.set('max_price', query.maxPrice);
    }
    if (query.minRating !== null) {
      params = params.set('min_rating', query.minRating);
    }
    params = params.set('sort_by', query.sortBy).set('order', query.order);

    this.http.get<PagedProductsResponse>(`${this.apiUrl}/products`, { params }).subscribe((response) => {
      this.products.set(response.items.map((item) => this.toProduct(item)));
      this.total.set(response.meta.total_items);
    });
  }

  getById(id: number): Observable<Product | null> {
    if (!id) {
      return of(null);
    }

    return this.http.get<ProductDto>(`${this.apiUrl}/products/${id}`).pipe(
      map((item) => this.toProduct(item)),
    );
  }

  private toProduct(item: ProductDto) {
    return new Product(
      item.id,
      item.title,
      item.images?.[0] ?? '',
      item.images ?? [],
      item.characteristics ?? {},
      item.price,
      item.rating,
      item.description,
      '',
    );
  }
}
