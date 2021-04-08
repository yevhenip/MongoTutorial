import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {DataService} from '../data.service';

@Injectable({
  providedIn: 'root'
})
export class CustomerService extends DataService {

  constructor(http: HttpClient) {
    super(http);
    this.url = 'http://localhost:2000/api/v1/customers/';
  }
}
