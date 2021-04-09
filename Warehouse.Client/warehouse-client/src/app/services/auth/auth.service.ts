import {Injectable} from '@angular/core';
import {JwtHelperService} from '@auth0/angular-jwt';
import {HttpClient, HttpHeaders, HttpRequest} from '@angular/common/http';
import {catchError} from 'rxjs/operators';
import {of} from 'rxjs';
import {LoginModel} from 'src/app/models/auth/loginModel';
import {RegisterModel} from 'src/app/models/auth/registerModel';
import { environment } from 'src/environments/environment';

@Injectable()
export class AuthService {

  private options = {headers: new HttpHeaders().set('Content-Type', 'application/json')};
  private url = environment.authApi;

  constructor(private http: HttpClient, private jwtService: JwtHelperService) {
  }

  login(loginModel: LoginModel) {
    let body = JSON.stringify(loginModel);
    return this.http.post(this.url + 'login', body, this.options)
      .pipe(catchError(err => of(err))).toPromise();
  }

  logout() {
    let options = {
      headers: new HttpHeaders()
        .set('Authorization', `Bearer ${this.jwtService.tokenGetter()}`)
    };
    this.http.post(this.url + 'logout', {}, options).subscribe();
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }

  register(registerModel: RegisterModel) {
    return this.http.post(this.url + 'register', registerModel)
      .pipe(catchError(err => of(err))).toPromise();
  }

  isLoggedIn() {
    return !this.jwtService.isTokenExpired();
  }

  isAdmin() {
    if (!this.isLoggedIn()) return false
    let user = this.jwtService.decodeToken()
    let roles: Array<string> = user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    return roles.indexOf('Admin') > -1;
  }

  initializeLocalStorage(authenticatedUser : any)
  {
    localStorage.setItem('jwtToken', authenticatedUser.jwtToken);
    localStorage.setItem('refreshToken', authenticatedUser.refreshToken);
    localStorage.setItem('user', JSON.stringify(authenticatedUser.user));
  }
}
