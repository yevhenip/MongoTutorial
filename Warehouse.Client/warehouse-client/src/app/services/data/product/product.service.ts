import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {JwtHelperService} from "@auth0/angular-jwt";
import {DataService} from '../data.service';
import {environment} from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService extends DataService {
  constructor(http: HttpClient) {
    super(http);
    this.url = environment.productApi;
  }
}
