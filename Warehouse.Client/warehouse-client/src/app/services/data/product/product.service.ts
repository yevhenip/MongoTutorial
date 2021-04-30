import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {JwtHelperService} from "@auth0/angular-jwt";
import {DataService} from '../data.service';
import {environment} from 'src/environments/environment';
import {of} from 'rxjs';
import {catchError} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ProductService extends DataService {
  constructor(http: HttpClient) {
    super(http, environment.productApi);
  }

  getFileForExport() {
    return this.http.get(this.url + 'export', {
      responseType: 'blob', headers: new HttpHeaders()
        .set('Authorization', `Bearer ${localStorage.getItem('jwtToken')}`)
    }).toPromise();
  }

  import(file: FormData){
    let options = {
      headers: new HttpHeaders()
        .set('Authorization', `Bearer ${localStorage.getItem('jwtToken')}`)
    };
    return this.http.post(this.url + 'import', file, this.options).pipe(catchError(err => of(err))).toPromise();
  }
}
