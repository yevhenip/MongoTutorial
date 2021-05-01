import {Component, OnInit, ViewChild} from '@angular/core';
import {Product} from '../models/product';
import {MatPaginator} from '@angular/material/paginator';
import {MatSort} from '@angular/material/sort';
import {MatTableDataSource} from '@angular/material/table';
import {MatDialog} from '@angular/material/dialog';
import {ProductFormComponent} from '../product-form/product-form.component';
import {JwtHelperService} from '@auth0/angular-jwt';
import {ProductService} from '../services/data/product/product.service';
import {AuthService} from '../services/auth/auth.service';
import {AdminPanelComponent} from '../admin-panel/admin-panel.component';
import {DataPanel} from '../abstract/data-panel';
import {exportFile} from '../utils/util';
import {LogService} from '../services/data/log/log.service';
import {Log} from '../models/log';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent extends DataPanel implements OnInit {

  fileName = '';

  constructor(private productService: ProductService, public authService: AuthService, public dialog: MatDialog,
              private jwtService: JwtHelperService, logService: LogService) {
    super(productService, logService);
  }

  async ngOnInit(): Promise<any> {
    super.ngOnInit();
    this.displayedColumns = ['name', 'dateOfReceipt', 'manufacturersName', 'customerName'];
    if (this.authService.isAdmin()) {
      this.displayedColumns.push('editDelete');
    }
    return Promise.resolve();
  }

  openDialogForCreation() {
    super.openDialogForCreation(this.dialog, ProductFormComponent, 'product');
  }

  openDialogForEditing(product: Product) {
    super.openDialogForEditing(product, this.dialog, ProductFormComponent, 'product');
  }

  async export() {
    let file = await this.productService.getFileForExport();
    exportFile(file, 'products');
  }

  async onFileSelected(event: any) {
    const file: File = event.target.files[0];

    this.fileName = file.name;
    const formData = new FormData();
    formData.append("file", file);

    let response = await this.productService.import(formData);
    if (response.error) {
      let message = response.error.text ? response.error.text : response.error.errors[Object.keys(response.error.errors)[0]];
      let errors = document.getElementById('errors');
      // @ts-ignore
      errors?.innerText = "";
      errors?.append(message);
    }
  }

  async openDialogForGrouping() {
    super.openDialogForGrouping(this.dialog);
  }
}
