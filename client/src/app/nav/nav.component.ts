import { Component } from '@angular/core';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent {
  model: any = {}

  constructor () {}

  ngOnInit(): void {

  }

  login() {
    console.log(this.model);
  }
}