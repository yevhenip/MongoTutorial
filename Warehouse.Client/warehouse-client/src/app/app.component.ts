import {Component} from '@angular/core';
import {User} from './models/user';
import {ProfileService} from './services/auth/profile.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  userName: string = this.profileService.decodeToken().UserName;

  constructor(private profileService: ProfileService) {
  }

  onActivate(componentReference: any) {
    componentReference.onUserLoggedIn?.subscribe((user: User) => {
      this.userName = user.userName;
    })
  }
}
