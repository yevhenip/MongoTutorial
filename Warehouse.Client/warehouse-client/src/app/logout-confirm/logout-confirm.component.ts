import {Component} from '@angular/core';
import {MatDialogRef} from "@angular/material/dialog";

@Component({
  selector: 'app-logout-confirm',
  templateUrl: './logout-confirm.component.html',
  styleUrls: ['./logout-confirm.component.css']
})
export class LogoutConfirmComponent {

  constructor(public dialogRef: MatDialogRef<LogoutConfirmComponent>) {
  }

  exit(value: boolean) {
    this.dialogRef.close(value);
  }
}
