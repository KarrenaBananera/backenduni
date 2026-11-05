import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { environment } from '../../environments/environment';

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
  private readonly storageKey = 'auth-token';

  readonly token = signal<string | null>(localStorage.getItem(this.storageKey));
  readonly email = signal<string | null>(null);

  register(email: string, password: string) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/users/register`, { email, password })
      .pipe(tap((response) => this.store(response)));
  }

  login(email: string, password: string) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/users/login`, { email, password })
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
