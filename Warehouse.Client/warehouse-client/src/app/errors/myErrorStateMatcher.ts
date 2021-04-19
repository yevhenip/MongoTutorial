import {FormControl, FormGroupDirective, NgForm} from "@angular/forms";
import {ErrorStateMatcher as StateMatcher} from "@angular/material/core";

export class ErrorStateMatcher implements StateMatcher {
  isErrorState(control: FormControl | null, form: FormGroupDirective | NgForm | null): boolean {
    const isSubmitted = form && form.submitted;
    return !!(control && control.invalid && (control.dirty || control.touched || isSubmitted));
  }
}
