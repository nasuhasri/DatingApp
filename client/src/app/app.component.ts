import { HttpClient } from '@angular/common/http';
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
  
  constructor(private http: HttpClient, private accountService: AccountService) {
    console.log('in constructor')
  }

  ngOnInit(): void {
    this.getUsers();
    this.setCurrentUser();
  }

  getUsers() {
    // add any initialization code that we want to do
    // http is injected via the constructor method and we use this keyword
    // returns observable object
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request completed')
    })
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
