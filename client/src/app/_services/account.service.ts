import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

// angular services can be injected into the components or other services
@Injectable({
  providedIn: 'root' // automatically provided in root modules
})

// responsible for making http req from client to server
// use service to centralize http req
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model);
  }
}
