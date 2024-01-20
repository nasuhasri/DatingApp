import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';
import { PresenceService } from './presence.service';

// angular services can be injected into the components or other services
@Injectable({
  providedIn: 'root' // automatically provided in root modules
})

// responsible for making http req from client to server
// use service to centralize http req
export class AccountService {
  baseUrl = environment.apiUrl;
  // this can be used outside of the account service and we set it to null
  private currentUserSource = new BehaviorSubject<User | null>(null);
  // $ - to signify that this is unobservable
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient, private presenceService: PresenceService) { }

  login(model: User) {
    // return type will be type of User
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;

        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }

  setCurrentUser(user: User) {
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);

    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);

    // create hub connection to our user
    this.presenceService.createHubConnection(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);

    this.presenceService.stopHubConnection();
  }

  getDecodedToken(token: string) {
    // return the second part of the token (can decode token via jwt.ms)
    return JSON.parse(atob(token.split(".")[1]))
  }
}
