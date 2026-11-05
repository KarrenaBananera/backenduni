import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { environment } from '../../environments/environment';
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
}

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private http = inject(HttpClient);

  public products = toSignal(
    this.http
      .get<PagedProductsResponse>(`${environment.apiUrl}/products?page=1&limit=100`)
      .pipe(map((response) => response.items.map((item) => this.toProduct(item)))),
    { initialValue: [] },
  );

  private toProduct(item: ProductDto) {
    return new Product(
      item.id,
      item.title,
      item.images[0] ?? '',
      item.images ?? [],
      item.characteristics ?? {},
      item.price,
      item.rating,
      item.description,
      '',
    );
  }
}
