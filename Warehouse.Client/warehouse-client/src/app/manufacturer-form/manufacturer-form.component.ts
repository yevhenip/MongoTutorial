import {Component, Inject} from '@angular/core';
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {MAT_DIALOG_DATA, MatDialogRef} from "@angular/material/dialog";
import {Manufacturer} from '../models/manufacturer';
import {MyErrorStateMatcher} from "../errors/myErrorStateMatcher";

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

  matcher = new MyErrorStateMatcher();

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
  }

  ok() {
    this.dialogRef.close(this.formControl.value);
  }
}
