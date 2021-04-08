import {Injectable} from '@angular/core';
import {JwtHelperService} from '@auth0/angular-jwt';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {catchError} from 'rxjs/operators';
import {of} from 'rxjs';
import {LoginModel} from 'src/app/models/auth/loginModel';
import {RegisterModel} from 'src/app/models/auth/registerModel';

@Injectable()
export class AuthService {

  private options = {headers: new HttpHeaders().set('Content-Type', 'application/json')};
  private url = 'http://localhost:1000/api/v1/auth/';

  constructor(private http: HttpClient, private jwtService: JwtHelperService) {
  }

  login(loginModel: LoginModel) {
    let body = JSON.stringify(loginModel);
    return this.http.post(this.url + 'login', body, this.options)
      .pipe(catchError(err => of(err)));
  }

  logout() {
    let options = {
      headers: new HttpHeaders()
        .set('Authorization', `Bearer ${localStorage.getItem('jwtToken')}`)
        .set('Content-Type', 'application/json')
    };
    this.http.post(this.url + 'logout', {}, options).subscribe();
  }

  register(registerModel: RegisterModel) {
    let body = JSON.stringify(registerModel);
    return this.http.post(this.url + 'register', body, this.options)
      .pipe(catchError(err => of(err)));
  }

  isLoggedIn() {
    return !this.jwtService.isTokenExpired();
  }

  isAdmin() {
    if (!this.isLoggedIn()) return false
    let user = this.jwtService.decodeToken()
    let roles: Array<string> = user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    if (roles.indexOf('Admin') > -1) return false;
    return true;
  }
}
