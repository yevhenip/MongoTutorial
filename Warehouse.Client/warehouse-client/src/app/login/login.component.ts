import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {MyErrorStateMatcher} from '../errors/myErrorStateMatcher';
import {User} from '../models/user';
import { AuthService } from '../services/auth/auth.service';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  matcher = new MyErrorStateMatcher();
  private returnUrl!: string;

  @Output() loginedUser = new EventEmitter<User>();

  formControl = new FormGroup({
    userName: new FormControl('', [Validators.required, Validators.minLength(5)],),
    password: new FormControl('', [Validators.required, Validators.minLength(5)])
  })

  constructor(private route: ActivatedRoute, private router: Router, private authService: AuthService) {
  }

  async login() {
    let login = this.formControl.value;
    let loginResponse: any;
    let response: any = await this.authService.login(login).toPromise();
    this.formControl.get('password')?.reset();

    if (response.error) {
      let controlName = Object.keys(response.error.Errors)[0];
      this.formControl.get(controlName)?.setErrors({serverError: true});
      return;
    }

    localStorage.setItem('jwtToken', response.jwtToken);
    localStorage.setItem('refreshToken', response.refreshToken);
    localStorage.setItem('user', JSON.stringify(response.user));
    this.loginedUser.emit(response.user);
    this.router.navigateByUrl(this.returnUrl);
  }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }
}
