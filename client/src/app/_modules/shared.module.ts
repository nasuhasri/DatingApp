import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(), // two way bindings
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right'
    }) // toastr for notification
  ],
  exports: [
    BsDropdownModule,
    ToastrModule
  ]
})
export class SharedModule { }
