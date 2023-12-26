import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // this shows this component will receive information from parent component
  @Input() usersFromHomeComponent: any; // parent to child component
  @Output() cancelRegister = new EventEmitter(); // child to parent component
  model: any = {}

  constructor() {}

  ngOnInit(): void {
  }

  register() {
    console.log(this.model);
  }

  cancel() {
    // emit value of false to turn off the register mode in home components
    this.cancelRegister.emit(false);
  }
}
