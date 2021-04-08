import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {DataService} from '../data.service';

@Injectable({
  providedIn: 'root'
})
export class ManufacturerService extends DataService {

  constructor(http: HttpClient) {
    super(http);
    this.url = 'http://localhost:3000/api/v1/manufacturers/';
  }
}
