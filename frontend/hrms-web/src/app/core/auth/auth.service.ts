import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { User } from '../models/user.model';
import { LoginRequest, LoginResponse } from '../models/auth-response.model';
import { UserRole } from '../models/role.enum';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  private tokenKey = 'access_token';
  private refreshTokenKey = 'refresh_token';

  constructor(private http: HttpClient) {
    // Load user from localStorage if token exists
    this.loadUserFromToken();
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    // Add ipAddress and userAgent if not provided
    const loginReq: LoginRequest = {
      ...credentials,
      ipAddress: this.getClientIpAddress(),
      userAgent: navigator.userAgent,
    };

    return this.http
      .post<LoginResponse>(
        `${environment.apiUrl}${environment.apiEndpoints.auth}/login`,
        loginReq
      )
      .pipe(
        tap((response) => {
          if (response.accessToken && response.employee) {
            // Store tokens
            localStorage.setItem(this.tokenKey, response.accessToken);
            localStorage.setItem(this.refreshTokenKey, response.refreshToken);

            // Create user object
            const user: User = {
              id: response.employee.id,
              email: response.employee.email,
              firstName: response.employee.firstName,
              lastName: response.employee.lastName,
              role: response.employee.role as UserRole,
            };
            this.currentUserSubject.next(user);
          }
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    this.currentUserSubject.next(null);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.refreshTokenKey);
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.getAccessToken() !== null;
  }

  hasRole(role: UserRole): boolean {
    const user = this.currentUserSubject.value;
    return user ? user.role === role : false;
  }

  hasAnyRole(roles: UserRole[]): boolean {
    const user = this.currentUserSubject.value;
    return user ? roles.includes(user.role) : false;
  }

  private loadUserFromToken(): void {
    const token = this.getAccessToken();
    if (token) {
      // TODO: Decode JWT token to get user info and set currentUserSubject
      // For now, you might want to call a profile endpoint
    }
  }

  private getClientIpAddress(): string {
    // TODO: If needed, get the actual client IP
    // For now, return a placeholder
    return '0.0.0.0';
  }
}
