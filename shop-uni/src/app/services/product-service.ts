import { inject, Injectable } from '@angular/core';
import { Product } from '../shared/models/product';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
    private productsUrl = 'products.json';
    private http = inject(HttpClient);

 public products = toSignal(this.http.get<Product[]>(this.productsUrl), {
  initialValue: [],
  });
}
