import {Component, Inject, OnInit, ViewChild} from '@angular/core';
import {MatTableDataSource} from "@angular/material/table";
import {Customer} from "../models/customer";
import {MatPaginator} from "@angular/material/paginator";
import {MatDialog} from '@angular/material/dialog';
import {DataService} from '../services/data/data.service';
import {ComponentType} from '@angular/cdk/portal';
import {Product} from "../models/product";
import {ProductFormComponent} from "../product-form/product-form.component";

@Component({
  selector: 'admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent {
  constructor() {
  }
}
