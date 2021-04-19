import {Component, OnInit, ViewChild} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {MatTableDataSource} from '@angular/material/table';
import {ManufacturerFormComponent} from '../manufacturer-form/manufacturer-form.component';
import {Manufacturer} from '../models/manufacturer';
import {MatPaginator} from "@angular/material/paginator";
import {ManufacturerService} from '../services/data/manufacturers/manufacturer.service';
import {AdminPanelComponent} from '../admin-panel/admin-panel.component';
import {DataPanel} from '../abstract/data-panel';
import {LogService} from '../services/data/log/log.service';

@Component({
  selector: 'manufacturer-panel',
  templateUrl: './manufacturer-panel.component.html',
  styleUrls: ['./manufacturer-panel.component.css']
})
export class ManufacturerPanelComponent extends DataPanel implements OnInit {

  constructor(private manufacturerService: ManufacturerService, public dialog: MatDialog, logService: LogService) {
    super(manufacturerService, logService);
  }

  ngOnInit(): Promise<any> {
    super.ngOnInit();
    this.displayedColumns = ['name', 'address', 'editDelete'];
    return Promise.resolve();
  }

  openDialogForCreation() {
    super.openDialogForCreation(this.dialog, ManufacturerFormComponent, 'manufacturer');
  }

  openDialogForEditing(manufacturer: Manufacturer) {
    super.openDialogForEditing(manufacturer, this.dialog, ManufacturerFormComponent, 'manufacturer');
  }
}
