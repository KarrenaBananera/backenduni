import { HttpClient, HttpParams } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { APP_CONFIG } from '../../app-config';

interface UserDto {
  id: string;
  userName: string;
  email: string;
  roles: string[];
  createdAt: string;
}

interface PagedUsersResponse {
  items: UserDto[];
  meta: {
    total_items: number;
    total_pages: number;
    current_page: number;
    limit: number;
  };
}

@Component({
  selector: 'app-admin-users-page',
  imports: [FormsModule],
  templateUrl: './admin-users-page.html',
  styleUrl: './admin-users-page.css',
})
export class AdminUsersPage implements OnInit {
  private http = inject(HttpClient);
  private apiUrl = inject(APP_CONFIG).apiUrl.replace(/\/$/, '');

  search = '';
  readonly pageSize = 10;
  page = signal(1);
  totalPages = signal(0);
  totalItems = signal(0);
  users = signal<UserDto[]>([]);
  error = signal<string | null>(null);

  ngOnInit() {
    this.load();
  }

  load(page = this.page()) {
    let params = new HttpParams().set('page', page).set('limit', this.pageSize);
    if (this.search.trim()) {
      params = params.set('search', this.search.trim());
    }

    this.http.get<PagedUsersResponse>(`${this.apiUrl}/users`, { params }).subscribe({
      next: (response) => {
        this.users.set(response.items);
        this.page.set(response.meta.current_page);
        this.totalPages.set(response.meta.total_pages);
        this.totalItems.set(response.meta.total_items);
        this.error.set(null);
      },
      error: () => this.error.set('Could not load users.'),
    });
  }

  searchUsers() {
    this.page.set(1);
    this.load(1);
  }

  previousPage() {
    if (this.page() > 1) {
      this.load(this.page() - 1);
    }
  }

  nextPage() {
    if (this.page() < this.totalPages()) {
      this.load(this.page() + 1);
    }
  }

  grantAdmin(user: UserDto) {
    this.http.post<UserDto>(`${this.apiUrl}/users/${user.id}/admin`, {}).subscribe({
      next: (updated) => {
        this.users.update((items) => items.map((item) => (item.id === updated.id ? updated : item)));
        this.error.set(null);
      },
      error: () => this.error.set('Could not grant admin role.'),
    });
  }

  isAdmin(user: UserDto) {
    return user.roles.includes('admin');
  }
}
