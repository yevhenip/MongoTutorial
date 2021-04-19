import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {environment} from 'src/environments/environment';
import {DataService} from '../data.service';

@Injectable({
  providedIn: 'root'
})
export class LogService extends DataService {

  constructor(http: HttpClient) {
    super(http, environment.logApi);
  }

  getActual() {
    this.options.headers = new HttpHeaders()
      .set('Authorization', `Bearer ${localStorage.getItem('jwtToken')}`);

    return this.http.get(this.url + 'actual', this.options).toPromise();
  }
}
