import {Component, Input} from '@angular/core';
import {User} from '../models/user';
import {HttpClient} from "@angular/common/http";
import {JwtHelperService} from "@auth0/angular-jwt";
import {Router} from '@angular/router';
import {AuthService} from '../services/auth/auth.service';

@Component({
  selector: 'navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  @Input() userName!: string;

  constructor(public authService: AuthService, private jwtService: JwtHelperService, private router: Router) {
  }

  logout() {
    this.authService.logout();
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    this.router.navigateByUrl('/login');
  }
}
