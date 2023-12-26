import { Component } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {
  model: any = {}

  constructor (public accountService: AccountService) {}

  ngOnInit(): void {
  }

  // getCurrentUser() {
  //   // bcs this is an observable, we must subscribe to something
  //   this.accountService.currentUser$.subscribe({
  //     // !! - turns user object to boolean
  //     next: user => this.loggedIn = !!user,
  //     error: error => console.log(error )
  //   })
  // }

  login() {
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
      },
      error: error => console.log(error)
    })
  }

  logout() {
    this.accountService.logout();
  }
}
