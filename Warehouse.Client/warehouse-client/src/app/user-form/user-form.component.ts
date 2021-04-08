import {Component, Inject} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {MyErrorStateMatcher} from "../errors/myErrorStateMatcher";
import {User} from "../models/user";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent {
  firstName!: string;
  lastName!: string;
  formControl = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.minLength(5)]),
    lastName: new FormControl('', Validators.minLength(5)),
    userName: new FormControl('', [Validators.required, Validators.minLength(5)]),
    email: new FormControl('', [Validators.required, Validators.email]),
    phone: new FormControl('', [Validators.required, Validators.pattern('^\\(?([0-9]{3})\\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$')])
  });

  matcher = new MyErrorStateMatcher();


  constructor(public dialogRef: MatDialogRef<UserFormComponent>, @Inject(MAT_DIALOG_DATA) public user: User) {
    let array = user.fullName.split(' ');
    this.firstName = array[0];
    this.lastName = array[1];
  }

  ok() {
    let user = this.formControl.value;
    user.fullName = user.firstName + ' ' + user?.lastName;
    this.dialogRef.close(user);
  }
}
