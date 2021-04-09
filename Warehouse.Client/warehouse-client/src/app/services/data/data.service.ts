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
  };

  getAll() {
    this.options.headers = new HttpHeaders()
      .set('Authorization', `Bearer ${localStorage.getItem('jwtToken')}`);

    return this.http.get(this.url, this.options).toPromise();
  }

  delete(id: string) {
    return this.http.delete(this.url + id, this.options).toPromise();
  }

  create(item: any) {
    return this.http.post(this.url, item, this.options);
  }

  edit(item: any) {
    return this.http.put(this.url + item.id, item, this.options);
  }
}
