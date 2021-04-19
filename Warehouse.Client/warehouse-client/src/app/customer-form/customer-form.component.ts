import {Component} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {ErrorStateMatcher} from "../errors/myErrorStateMatcher";
import {Customer} from "../models/customer" ;
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import { PHONE_PATTERN } from '../utils/util';

@Component({
  selector: 'app-customer-form',
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.css']
})
export class CustomerFormComponent {
  firstName!: string;
  lastName!: string;
  formControl = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.minLength(5)]),
    lastName: new FormControl('', Validators.minLength(5)),
    email: new FormControl('', [Validators.required, Validators.email]),
    phone: new FormControl('', [Validators.required, Validators.pattern(PHONE_PATTERN)])
  });

  matcher = new ErrorStateMatcher();

  customer!: Customer;

  constructor(public dialogRef: MatDialogRef<CustomerFormComponent>) {
  }

  ok() {
    let customer = this.formControl.value;
    customer.fullName = customer.firstName + ' ' + customer?.lastName;
    this.dialogRef.close(customer);
  }
}
