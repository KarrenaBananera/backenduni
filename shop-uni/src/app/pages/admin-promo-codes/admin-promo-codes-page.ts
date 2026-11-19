import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { APP_CONFIG } from '../../app-config';

interface PromoCodeDto {
  id: string;
  code: string;
  discountPercentage: number;
  isActive: boolean;
}

@Component({
  selector: 'app-admin-promo-codes-page',
  imports: [FormsModule],
  templateUrl: './admin-promo-codes-page.html',
  styleUrl: './admin-promo-codes-page.css',
})
export class AdminPromoCodesPage implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = inject(APP_CONFIG).apiUrl.replace(/\/$/, '');

  code = '';
  discountPercentage: number | null = null;
  promoCodes = signal<PromoCodeDto[]>([]);
  error = signal<string | null>(null);

  ngOnInit() {
    this.load();
  }

  load() {
    this.http.get<PromoCodeDto[]>(`${this.apiUrl}/promo-codes`).subscribe({
      next: (promoCodes) => {
        this.promoCodes.set(promoCodes);
        this.error.set(null);
      },
      error: () => this.error.set('Could not load promo codes.'),
    });
  }

  add() {
    this.http
      .post<PromoCodeDto>(`${this.apiUrl}/promo-codes`, {
        code: this.code,
        discountPercentage: this.discountPercentage,
      })
      .subscribe({
        next: () => {
          this.code = '';
          this.discountPercentage = null;
          this.load();
        },
        error: (error: HttpErrorResponse) => {
          this.error.set(error.error?.error ?? 'Could not add promo code.');
        },
      });
  }

  remove(promoCode: PromoCodeDto) {
    this.http.delete(`${this.apiUrl}/promo-codes/${promoCode.id}`).subscribe({
      next: () => this.promoCodes.update((items) => items.filter((item) => item.id !== promoCode.id)),
      error: () => this.error.set('Could not delete promo code.'),
    });
  }
}
