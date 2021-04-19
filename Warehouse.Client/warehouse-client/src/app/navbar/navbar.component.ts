import {Component, Input} from '@angular/core';
import {User} from '../models/user';
import {HttpClient} from "@angular/common/http";
import {Router} from '@angular/router';
import {AuthService} from '../services/auth/auth.service';
import {MatDialog} from "@angular/material/dialog";
import {LogoutConfirmComponent} from '../logout-confirm/logout-confirm.component';

@Component({
  selector: 'navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

  @Input() userName!: string;

  constructor(public authService: AuthService, private router: Router, private dialog: MatDialog) {
  }

  logout() {
    let dialog = this.dialog.open(LogoutConfirmComponent);
    dialog.afterClosed().subscribe(result => {
      if (result) {
        this.authService.logout();
        this.router.navigateByUrl('/login');
      }
    });
  }
}
