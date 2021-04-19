import {Component, Inject} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {Manufacturer} from '../models/manufacturer';
import {ErrorStateMatcher} from "../errors/myErrorStateMatcher";

@Component({
  selector: 'app-manufacturer-form',
  templateUrl: './manufacturer-form.component.html',
  styleUrls: ['./manufacturer-form.component.css']
})
export class ManufacturerFormComponent {
  formControl = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(5)]),
    address: new FormControl('', Validators.required),
  });

  matcher = new ErrorStateMatcher();

  manufacturer!: Manufacturer;

  constructor(public dialogRef: MatDialogRef<ManufacturerFormComponent>, @Inject(MAT_DIALOG_DATA) manufacturer: Manufacturer) {
    if (manufacturer) {
      this.manufacturer = manufacturer;
    } else {
      this.manufacturer = {
        name: '',
        address: ''
      } as unknown as Manufacturer;
    }
    this.formControl.get('name')?.setValue(this.manufacturer.name);
    this.formControl.get('address')?.setValue(this.manufacturer.address);
  }

  ok() {
    this.dialogRef.close(this.formControl.value);
  }
}
