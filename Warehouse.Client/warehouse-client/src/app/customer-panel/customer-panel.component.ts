import {Component, OnInit, ViewChild} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {MatTableDataSource} from '@angular/material/table';
import {CustomerFormComponent} from '../customer-form/customer-form.component';
import {Customer} from '../models/customer';
import {MatPaginator} from "@angular/material/paginator";
import {CustomerService} from '../services/data/customer/customer.service';
import {AdminPanelComponent} from '../admin-panel/admin-panel.component';
import {DataPanel} from '../abstract/data-panel';
import { LogService } from '../services/data/log/log.service';

@Component({
  selector: 'customer-panel',
  templateUrl: './customer-panel.component.html',
  styleUrls: ['./customer-panel.component.css']
})
export class CustomerPanelComponent extends DataPanel implements OnInit {

  constructor(private customerService: CustomerService, public dialog: MatDialog, logService: LogService) {
    super(customerService, logService);
  }

  ngOnInit(): Promise<any> {
    super.ngOnInit()
    this.displayedColumns = ['fullName', 'email', 'phone', 'editDelete'];
    return Promise.resolve();
  }

  openDialogForCreation() {
    super.openDialogForCreation(this.dialog, CustomerFormComponent, 'customer');
  }
}
