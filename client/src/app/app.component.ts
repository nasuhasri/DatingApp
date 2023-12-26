import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

// A lifecycle hook that is called after Angular has initialized all data-bound properties of a directive
// to  handle any additional initialization tasks.
export class AppComponent implements OnInit {
  title = 'Dating App';
  users: any; // declaring variable like javascript
  
  constructor(private accountService: AccountService) {
    console.log('in constructor')
  }

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser() {
    // ! - turns off typescript safety 
    // const user: User = JSON.parse(localStorage.getItem('user')!);

    const UserString = localStorage.getItem('user');

    if (!UserString) return;

    const user: User = JSON.parse(UserString);

    this.accountService.setCurrentUser(user);
  }

}
