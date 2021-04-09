import {Component} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {Router} from '@angular/router';
import {ErrorStateMatcher} from "../errors/myErrorStateMatcher";
import {RegisterModel} from '../models/auth/registerModel';
import {AuthService} from '../services/auth/auth.service';
import {PHONE_PATTERN} from '../utils/util';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  matcher = new ErrorStateMatcher();
  formControl = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.minLength(5)],),
    lastName: new FormControl('', [Validators.minLength(5)],),
    userName: new FormControl('', [Validators.required, Validators.minLength(5)],),
    password: new FormControl('', [Validators.required, Validators.minLength(5)]),
    confirmedPassword: new FormControl(''),
    email: new FormControl('', [Validators.required, Validators.email]),
    phone: new FormControl('', [Validators.required, Validators.pattern(PHONE_PATTERN)])
  });

  constructor(private router: Router, private authService: AuthService) {
  }

  async register() {
    let register: RegisterModel = this.formControl.value;
    register.fullName = this.formControl.value.firstName + this.formControl.value.lastName
    let registerResponse: any;

    if (register.password !== register.confirmedPassword) {
      this.formControl.get('confirmedPassword')?.setErrors({notConfirmedPassword: true});
      return;
    }

    let response = await this.authService.register(register);
    if (response.error) {
      let controlName = Object.keys(response.error.Errors)[0];
      this.formControl.get(controlName)?.setErrors({serverError: true});
      return;
    }

    this.router.navigateByUrl('/login');
  }
}
