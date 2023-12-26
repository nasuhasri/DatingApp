import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';

// angular services can be injected into the components or other services
@Injectable({
  providedIn: 'root' // automatically provided in root modules
})

// responsible for making http req from client to server
// use service to centralize http req
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';
  // this can be used outside of the account service and we set it to null
  private currentUserSource = new BehaviorSubject<User | null>(null);
  // $ - to signify that this is unobservable
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model: User) {
    // return type will be type of User
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;

        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }

        // if we need to use value from the inside function, we need to return inside the map also
        return user;
      })
    )
  }

  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}
