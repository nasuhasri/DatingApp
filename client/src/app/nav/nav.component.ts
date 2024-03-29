import { Component } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {
  model: any = {}

  // Router - A service that provides navigation among views and URL manipulation capabilities.
  constructor (public accountService: AccountService, private router: Router, private toastr: ToastrService) {}

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
      next: _ => {
        this.router.navigateByUrl('/members');
        // reset login form
        this.model = {}
      },
    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}
