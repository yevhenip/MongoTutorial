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

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent extends AdminPanelComponent implements OnInit {

  displayedColumns: string[] = ['name', 'dateOfReceipt', 'manufacturersName', 'customerName'];

  constructor(private productService: ProductService, public authService: AuthService, public dialog: MatDialog,
              private jwtService: JwtHelperService) {
    super(productService);
  }


  ngOnInit() : Promise<any> {
    super.ngOnInit();
    this.displayedColumns = ['name', 'dateOfReceipt', 'manufacturersName', 'customerName'];
    if (this.authService.isAdmin()) {
      this.displayedColumns.push('editDelete');
    }
    return Promise.resolve();
  }

  openDialogForCreation() {
    super.openDialogForCreation(this.dialog, ProductFormComponent);
  }

  openDialogForEditing(product: Product) {
    super.openDialogForEditing(product, this.dialog, ProductFormComponent);
  }
}
