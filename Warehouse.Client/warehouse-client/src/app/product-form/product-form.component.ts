import {Component, Inject, OnInit} from '@angular/core';
import {MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import {Product} from '../models/product';
import {MyErrorStateMatcher} from "../errors/myErrorStateMatcher";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {Manufacturer} from '../models/manufacturer';
import {Customer} from '../models/customer';
import {CustomerService} from '../services/data/customer/customer.service';
import {ManufacturerService} from '../services/data/manufacturers/manufacturer.service';

@Component({
  selector: 'product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']
})
export class ProductFormComponent implements OnInit {
  manufacturerIds: string[] = [];
  customerId!: string;
  product!: Product;
  myFilter = (d: Date | null): boolean => {
    const date = (d || new Date());
    return new Date().getTime() > date.getTime();
  }
  matcher = new MyErrorStateMatcher();

  formControl = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(5)]),
    dateOfReceipt: new FormControl('', Validators.required),
    manufacturerIds: new FormControl(''),
    customerId: new FormControl('')
  });

  manufacturers!: Manufacturer[];
  customers!: Customer[];

  constructor(public dialogRef: MatDialogRef<ProductFormComponent>, private manufacturerService: ManufacturerService,
              private customerService: CustomerService, @Inject(MAT_DIALOG_DATA) product: Product) {
    if (product) {
      this.product = product;
      for (let manufacturer of product.manufacturers) {
        this.manufacturerIds.push(manufacturer.id);
      }
      this.customerId = product.customer?.id;
    } else {
      this.product = {
        name: '',
        id: '',
        customer: {id: ''},
        manufacturers: [],
        dateOfReceipt: ''
      } as unknown as Product;
    }
  }

  ngOnInit(): void {
    this.manufacturerService.getAll().subscribe(response => this.manufacturers = response as Manufacturer[]);
    this.customerService.getAll().subscribe(response => this.customers = response as Customer[]);
    this.formControl.get('manufacturerIds')?.markAsTouched();
    this.formControl.get('customerId')?.markAsTouched();
  }


  ok() {
    if (!this.formControl.value.manufacturerIds) {
      this.formControl.value.manufacturerIds = [];
    }
    if (!this.formControl.value.dateOfReceipt) {
      this.formControl.value.dateOfReceipt = null;
    }
    this.dialogRef.close(this.formControl.value);
  }
}
