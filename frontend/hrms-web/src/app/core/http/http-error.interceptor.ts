import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable()
export class HttpErrorInterceptor implements HttpInterceptor {
  constructor(private router: Router) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          // TODO: Show toast/notification
          this.router.navigate(['/auth/login']);
        } else if (error.status === 403) {
          // TODO: Show forbidden error
          this.router.navigate(['/dashboard']);
        } else if (error.status >= 500) {
          // TODO: Show server error notification
        }

        return throwError(() => error);
      })
    );
  }
}
