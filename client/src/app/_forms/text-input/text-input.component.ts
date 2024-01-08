import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})

/**
 * ControlValueAccessor:
 *  - Defines an interface that acts as a bridge between the Angular forms API and a native element in the DOM.
 *  - Implement this interface to create a custom form control directive that integrates with Angular forms.
 */

export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  // given it an initial value which can be overridden by simply passing the type as a parameter.
  // so that we do not have to pass a type unless we want to use something other than text.
  @Input() type = 'text';

  /**
   * why we use @Self decorator?
   *  - when we inject something into constructor, its going to check if its been used recently, and if it has, its going to reuse that thing that it's kept in memory
   *  - when it comes to our input, we dont want to reuse another control that was already in memory
   *  - want to ensure the ngControl is unique to the input that we are updating in the DOM
   *  - and the way we ensure that is by using self decorator
   * 
   * NgControl: A base class that all FormControl-based directives extend. It binds a FormControl object to a DOM element.
   */
  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this; // this keyword represent text input component class
  }

  writeValue(obj: any): void {
  }

  registerOnChange(fn: any): void {
  }

  registerOnTouched(fn: any): void {
  }

  get control(): FormControl {
    // casting ngControl.control into FormControl
    return this.ngControl.control as FormControl;
  }
}
