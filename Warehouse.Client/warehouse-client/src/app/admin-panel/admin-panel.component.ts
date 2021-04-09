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
  dataSource!: MatTableDataSource<Object>;
  resultsLength!: number;
  displayedColumns!: string[]

  constructor(@Inject(DataService) public service: DataService) {
  }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  async ngOnInit() {
    let response = await this.service.getAll();
    let data = response as [];
    this.resultsLength = data.length;
    this.dataSource = new MatTableDataSource<Object>(data);
    this.dataSource.paginator = this.paginator;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
    this.dataSource.paginator?.firstPage();
  }

  openDialogForCreation(dialogItem: MatDialog, component: ComponentType<any>) {
    let dialog = dialogItem.open(component);
    dialog.afterClosed().subscribe(result => {
      if (result) {
        this.service.create(result).subscribe(creationResult => {
          this.dataSource.data.push(creationResult);
          this.dataSource._updateChangeSubscription();
        });
      }
    });
  }

  openDialogForEditing(item: any, dialogItem: MatDialog, component: ComponentType<any>) {
    let dialog = dialogItem.open(component, {
      data: JSON.parse(JSON.stringify(item))
    });
    dialog.afterClosed().subscribe(result => {
      if (result) {
        result.id = item.id;
        this.service.edit(result).subscribe(editResult => {
          let index = this.dataSource.data.indexOf(item);
          this.dataSource.data.fill(editResult, index, index + 1);
          this.dataSource._updateChangeSubscription();
        });
      }
    });
  }

  delete(item: any) {
    this.service.delete(item.id);
    this.dataSource.data.splice(this.dataSource.data.indexOf(item), 1);
    this.dataSource._updateChangeSubscription();
  }
}
