import {Inject, Injectable, Optional} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class DataService {
  url!: string;

  constructor(private http: HttpClient) {
  }

  options = {
    headers: new HttpHeaders()
      .set('Authorization', `Bearer ${localStorage.getItem('jwtToken')}`)
      .set('Content-Type', 'application/json')
      .set('Access-Control-Allow-Origin', '')
  };

  getAll() {
    return this.http.get(this.url, this.options);
  }

  delete(id: string) {
    return this.http.delete(this.url + id, this.options).subscribe();
  }

  create(item: any) {
    let body = JSON.stringify(item);
    return this.http.post(this.url, body, this.options);
  }

  edit(item: any) {
    let body = JSON.stringify(item);
    return this.http.put(this.url + item.id, body, this.options);
  }
}
