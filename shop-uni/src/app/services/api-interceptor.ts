import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth-service';

export const apiInterceptor: HttpInterceptorFn = (request, next) => {
  const auth = inject(AuthService);
  const jwt = auth.token();
  const headers = jwt ? request.headers.set('Authorization', `Bearer ${jwt}`) : request.headers;

  return next(request.clone({ headers, withCredentials: true }));
};
