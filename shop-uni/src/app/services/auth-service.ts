import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
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
  readonly email = signal<string | null>(this.readEmail(this.token()));
  readonly roles = signal<string[]>(this.readRoles(this.token()));
  readonly isAdmin = computed(() => this.roles().includes('admin'));
  readonly isSignedIn = computed(() => this.token() !== null);

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
    this.roles.set([]);
    localStorage.removeItem(this.storageKey);
  }

  private store(response: AuthResponse) {
    this.token.set(response.token);
    this.email.set(response.email);
    this.roles.set(response.roles ?? this.readRoles(response.token));
    localStorage.setItem(this.storageKey, response.token);
  }

  private readEmail(token: string | null): string | null {
    const email = this.readClaim(token, [
      'email',
      'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress',
    ]);
    return typeof email === 'string' ? email : null;
  }

  private readRoles(token: string | null): string[] {
    const role = this.readClaim(token, [
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
      'role',
    ]);

    if (Array.isArray(role)) {
      return role.filter((item): item is string => typeof item === 'string');
    }

    return typeof role === 'string' ? [role] : [];
  }

  private readClaim(token: string | null, names: string[]): unknown {
    if (!token) {
      return null;
    }

    try {
      const payload = token.split('.')[1];
      if (!payload) {
        return null;
      }

      const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
      const claims = JSON.parse(json) as Record<string, unknown>;
      return names.map((name) => claims[name]).find((value) => value !== undefined) ?? null;
    } catch {
      return null;
    }
  }
}
