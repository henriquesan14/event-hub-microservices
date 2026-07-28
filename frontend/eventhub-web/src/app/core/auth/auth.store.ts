import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, map, Observable, of, switchMap, tap } from 'rxjs';
import { AuthResponse, User } from '../api/models';

@Injectable({ providedIn: 'root' })
export class AuthStore {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly currentUser = signal<User | null>(null);
  private readonly initialized = signal(false);

  readonly user = this.currentUser.asReadonly();
  readonly isReady = this.initialized.asReadonly();
  readonly isAuthenticated = computed(() => this.currentUser() !== null);
  readonly isAdmin = computed(() => {
    const role = this.currentUser()?.role;
    return role === 'Admin' || role === 0;
  });
  readonly isOrganizer = computed(() => {
    const role = this.currentUser()?.role;
    return role === 'Organizer' || role === 2;
  });

  constructor() {
    this.loadCurrentUser().subscribe();
  }

  loadCurrentUser(): Observable<User | null> {
    return this.http.get<User>('/api/users/me').pipe(
      tap(user => this.currentUser.set(user)),
      map(user => user),
      catchError(() => {
        this.currentUser.set(null);
        return of(null);
      }),
      finalize(() => this.initialized.set(true)),
    );
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>('/api/auth', { email, password }).pipe(
      switchMap(response =>
        this.http.get<User>('/api/users/me').pipe(
          tap(user => this.currentUser.set(user)),
          map(() => response),
        ),
      ),
    );
  }

  register(name: string, email: string, password: string): Observable<User> {
    return this.http.post<User>('/api/auth/register', { name, email, password });
  }

  logout(): void {
    this.http.post<void>('/api/auth/logout', {}).pipe(
      finalize(() => {
        this.currentUser.set(null);
        void this.router.navigateByUrl('/');
      }),
    ).subscribe();
  }

  setUser(user: User): void {
    this.currentUser.set(user);
  }
}
