import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {DataService} from '../data.service';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ManufacturerService extends DataService {

  constructor(http: HttpClient) {
    super(http);
    this.url = environment.manufacturerApi;
  }
}
