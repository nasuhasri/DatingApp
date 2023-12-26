import { Component } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {
  model: any = {}
  loggedIn = false;

  constructor (private accountService: AccountService) {}

  ngOnInit(): void {
    this.getCurrentUser();
  }

  getCurrentUser() {
    // bcs this is an observable, we must subscribe to something
    this.accountService.currentUser$.subscribe({
      // !! - turns user object to boolean
      next: user => this.loggedIn = !!user,
      error: error => console.log(error )
    })
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: response => {
        console.log(response);
        this.loggedIn = true
      },
      error: error => console.log(error)
    })
  }

  logout() {
    this.accountService.logout();
    this.loggedIn = false;
  }
}
