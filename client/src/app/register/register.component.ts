import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // this shows this component will receive information from parent component
  @Input() usersFromHomeComponent: any;
  model: any = {}

  constructor() {}

  ngOnInit(): void {
  }

  register() {
    console.log(this.model);
  }

  cancel() {
    console.log('cancelled');
  }
}
