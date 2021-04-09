import {Component, OnInit, ViewChild} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import { AdminPanelComponent } from '../admin-panel/admin-panel.component';
import {User} from '../models/user';
import {UserService} from '../services/data/user/user.service';
import {UserFormComponent} from '../user-form/user-form.component';

@Component({
  selector: 'user-panel',
  templateUrl: './user-panel.component.html',
  styleUrls: ['./user-panel.component.css']
})
export class UserPanelComponent extends AdminPanelComponent implements OnInit {

  constructor(private userService: UserService, public dialog: MatDialog) {
    super(userService);
  }

  ngOnInit() : Promise<any> {
    super.ngOnInit();
    this.displayedColumns = ['fullName', 'userName', 'email', 'phone', 'registrationDateTime', 'editDelete'];
    return Promise.resolve();
  }

  openDialogForEditing(user: User) {
    super.openDialogForEditing(user, this.dialog, UserFormComponent);
  }
}
