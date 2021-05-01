import {Component, Inject} from '@angular/core';
import {MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import {GroupedData} from '../models/GroupedData';

@Component({
  selector: 'app-group-data',
  templateUrl: './group-data.component.html',
  styleUrls: ['./group-data.component.css']
})
export class GroupDataComponent {

  constructor(public dialogRef: MatDialogRef<GroupDataComponent>, @Inject(MAT_DIALOG_DATA) public groupedData: GroupedData[]) {
  }
}
