import {Inject, Injectable, Optional} from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class DataService {
  constructor(protected http: HttpClient, @Inject('url') protected url: string) {
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

  getPage(page: number, pageSize: number) {
    this.options.headers = new HttpHeaders()
      .set('Authorization', `Bearer ${localStorage.getItem('jwtToken')}`);

    return this.http.get(this.url + page + '/' + pageSize, this.options).toPromise();
  }

  delete(id: string) {
    return this.http.delete(this.url + id, this.options).toPromise();
  }

  create(item: any) {
    return this.http.post(this.url, item, this.options).toPromise();
  }

  edit(item: any) {
    return this.http.put(this.url + item.id, item, this.options).toPromise();
  }

  getCount() {
    return this.http.get(this.url + 'count', this.options).toPromise();
  }

  groupBy(){
    return this.http.get(this.url + 'group', this.options).toPromise();
  }
}
