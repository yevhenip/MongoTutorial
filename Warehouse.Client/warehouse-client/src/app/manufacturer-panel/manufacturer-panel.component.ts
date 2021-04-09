import {Component, OnInit, ViewChild} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {MatTableDataSource} from '@angular/material/table';
import {ManufacturerFormComponent} from '../manufacturer-form/manufacturer-form.component';
import {Manufacturer} from '../models/manufacturer';
import {MatPaginator} from "@angular/material/paginator";
import {ManufacturerService} from '../services/data/manufacturers/manufacturer.service';
import {AdminPanelComponent} from '../admin-panel/admin-panel.component';

@Component({
  selector: 'manufacturer-panel',
  templateUrl: './manufacturer-panel.component.html',
  styleUrls: ['./manufacturer-panel.component.css']
})
export class ManufacturerPanelComponent extends AdminPanelComponent implements OnInit {

  constructor(private manufacturerService: ManufacturerService, public dialog: MatDialog) {
    super(manufacturerService);
  }

  ngOnInit() : Promise<any> {
    super.ngOnInit();
    this.displayedColumns = ['name', 'address', 'editDelete'];
    return Promise.resolve();
  }

  openDialogForCreation() {
    super.openDialogForCreation(this.dialog, ManufacturerFormComponent);
  }

  openDialogForEditing(manufacturer: Manufacturer) {
    super.openDialogForEditing(manufacturer, this.dialog, ManufacturerFormComponent);
  }
}
