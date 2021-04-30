import {Component, OnInit, ViewChild} from '@angular/core';
import {MatDialog} from '@angular/material/dialog';
import {MatPaginator} from '@angular/material/paginator';
import {MatTableDataSource} from '@angular/material/table';
import {DataPanel} from '../abstract/data-panel';
import {AdminPanelComponent} from '../admin-panel/admin-panel.component';
import {User} from '../models/user';
import {LogService} from '../services/data/log/log.service';
import {UserService} from '../services/data/user/user.service';
import {UserFormComponent} from '../user-form/user-form.component';

@Component({
  selector: 'user-panel',
  templateUrl: './user-panel.component.html',
  styleUrls: ['./user-panel.component.css']
})
export class UserPanelComponent extends DataPanel implements OnInit {

  isMaster!: boolean;

  constructor(private userService: UserService, public dialog: MatDialog, logService: LogService) {
    super(userService, logService);
  }

  ngOnInit(): Promise<any> {
    super.ngOnInit();
    this.displayedColumns = ['fullName', 'userName', 'email', 'phone', 'registrationDateTime', 'editDelete'];
   this.isMaster = (JSON.parse(localStorage.getItem('user') as string) as User).roles.includes('Dungeon master');
    if (this.isMaster) {
      this.displayedColumns.push('makeAdmin')
    }
    return Promise.resolve();
  }

  openDialogForEditing(user: User) {
    super.openDialogForEditing(user, this.dialog, UserFormComponent, 'user');
  }

  makeAdmin(user: User) {
    user.roles.push('Admin');
    this.userService.makeAdmin(user.id);
  }
}
