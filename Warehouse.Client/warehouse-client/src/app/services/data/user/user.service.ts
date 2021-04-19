import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {DataService} from '../data.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService extends DataService {
  constructor(http: HttpClient) {
    super(http, environment.userApi);
  }
}
