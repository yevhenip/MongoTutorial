import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {DataService} from '../data.service';

@Injectable({
  providedIn: 'root'
})
export class UserService extends DataService {
  constructor(http: HttpClient) {
    super(http);
    this.url = 'http://localhost:7000/api/v1/users/';
  }
}
