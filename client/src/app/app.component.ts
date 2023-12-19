import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

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
  
  constructor(private http: HttpClient) {
    console.log('in constructor')
  }

  ngOnInit(): void {
    // add any initialization code that we want to do
    // http is injected via the constructor method and we use this keyword
    // returns observable object
    this.http.get('https://localhost:5000/api/users').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request completed')
    })
  }

}
