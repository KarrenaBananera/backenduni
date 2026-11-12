import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { APP_CONFIG } from '../app-config';

export interface AuthResponse {
  token: string;
  expiresAt: string;
  email: string;
  roles: string[];
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = inject(APP_CONFIG).apiUrl.replace(/\/$/, '');
  private readonly storageKey = 'auth-token';

  readonly token = signal<string | null>(localStorage.getItem(this.storageKey));
  readonly email = signal<string | null>(null);

  register(email: string, password: string) {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/users/register`, { email, password })
      .pipe(tap((response) => this.store(response)));
  }

  login(email: string, password: string) {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/users/login`, { email, password })
      .pipe(tap((response) => this.store(response)));
  }

  logout() {
    this.token.set(null);
    this.email.set(null);
    localStorage.removeItem(this.storageKey);
  }

  private store(response: AuthResponse) {
    this.token.set(response.token);
    this.email.set(response.email);
    localStorage.setItem(this.storageKey, response.token);
  }
}
