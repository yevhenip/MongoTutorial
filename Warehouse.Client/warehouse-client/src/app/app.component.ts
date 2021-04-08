import {Component} from '@angular/core';
import {User} from './models/user';
import {JwtHelperService} from "@auth0/angular-jwt";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  userName: string = this.jwtService.decodeToken()?.UserName;

  constructor(private jwtService: JwtHelperService) {
  }

  onActivate(componentReference: any) {
    componentReference.loginedUser?.subscribe((user: User) => {
      this.userName = user.userName;
    })
  }
}
