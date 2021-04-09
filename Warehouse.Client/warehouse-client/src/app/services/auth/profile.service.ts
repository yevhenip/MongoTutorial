import {Injectable} from '@angular/core';
import {JwtHelperService} from '@auth0/angular-jwt';
import {User} from 'src/app/models/user';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {

  constructor(private jwtService: JwtHelperService) {
  }

  decodeToken() {
    return this.jwtService.decodeToken();
  }
}
