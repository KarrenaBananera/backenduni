import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { APP_CONFIG } from '../../app-config';

interface ProductDto {
  id: number;
}

interface CharacteristicRow {
  key: string;
  value: string;
}

@Component({
  selector: 'app-admin-product-page',
  imports: [FormsModule],
  templateUrl: './admin-product-page.html',
  styleUrl: './admin-product-page.css',
})
export class AdminProductPage {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = inject(APP_CONFIG).apiUrl.replace(/\/$/, '');

  title = '';
  description = '';
  price: number | null = null;
  rating = 0;
  stockQuantity = 0;
  imageUrl = '';
  characteristics: CharacteristicRow[] = [{ key: '', value: '' }];

  images = signal<string[]>([]);
  error = signal<string | null>(null);
  pending = signal(false);
  uploading = signal(false);
  private uploadsInFlight = 0;

  addImageUrl() {
    const url = this.imageUrl.trim();
    if (!this.isHttpUrl(url)) {
      this.error.set('Image URL must start with http:// or https://.');
      return;
    }

    this.images.update((items) => [...items, url]);
    this.imageUrl = '';
    this.error.set(null);
  }

  onFiles(event: Event) {
    const input = event.target as HTMLInputElement;
    const files = Array.from(input.files ?? []);
    input.value = '';
    for (const file of files) {
      this.upload(file);
    }
  }

  removeImage(index: number) {
    this.images.update((items) => items.filter((_, itemIndex) => itemIndex !== index));
  }

  addCharacteristic() {
    this.characteristics = [...this.characteristics, { key: '', value: '' }];
  }

  removeCharacteristic(index: number) {
    this.characteristics = this.characteristics.filter((_, itemIndex) => itemIndex !== index);
    if (this.characteristics.length === 0) {
      this.characteristics = [{ key: '', value: '' }];
    }
  }

  add(form: NgForm) {
    if (form.invalid || this.pending() || this.uploading()) {
      return;
    }

    if (this.images().length < 3) {
      this.error.set('Add at least three images.');
      return;
    }

    const rows = this.characteristics
      .map((row) => ({ key: row.key.trim(), value: row.value.trim() }))
      .filter((row) => row.key || row.value);

    if (rows.some((row) => !row.key || !row.value)) {
      this.error.set('Each specification needs a name and a value.');
      return;
    }

    this.pending.set(true);
    this.http
      .post<ProductDto>(`${this.apiUrl}/products`, {
        title: this.title.trim(),
        description: this.description.trim(),
        price: this.price,
        rating: this.rating,
        images: this.images(),
        characteristics: Object.fromEntries(rows.map((row) => [row.key, row.value])),
        stockQuantity: this.stockQuantity,
      })
      .subscribe({
        next: (product) => {
          this.router.navigate(['/products', product.id]);
        },
        error: (error: HttpErrorResponse) => {
          this.pending.set(false);
          this.error.set(this.message(error));
        },
      });
  }

  private upload(file: File) {
    if (file.size > 5 * 1024 * 1024) {
      this.error.set('Image must be 5 MB or smaller.');
      return;
    }

    const body = new FormData();
    body.append('file', file);
    this.uploadsInFlight++;
    this.uploading.set(true);
    this.http.post<{ url: string }>(`${this.apiUrl}/images`, body).subscribe({
      next: (result) => {
        this.images.update((items) => [...items, result.url]);
        this.finishUpload();
        this.error.set(null);
      },
      error: (error: HttpErrorResponse) => {
        this.finishUpload();
        this.error.set(error.error?.error ?? 'Could not upload image.');
      },
    });
  }

  private finishUpload() {
    this.uploadsInFlight = Math.max(0, this.uploadsInFlight - 1);
    this.uploading.set(this.uploadsInFlight > 0);
  }

  private message(error: HttpErrorResponse) {
    const body = error.error;
    if (typeof body?.error === 'string') {
      return body.error;
    }

    const details = body?.errors;
    if (details && typeof details === 'object') {
      const first = Object.values(details).flat()[0];
      if (typeof first === 'string') {
        return first;
      }
    }

    return 'Could not add product.';
  }

  private isHttpUrl(value: string) {
    try {
      const url = new URL(value);
      return url.protocol === 'http:' || url.protocol === 'https:';
    } catch {
      return false;
    }
  }
}
