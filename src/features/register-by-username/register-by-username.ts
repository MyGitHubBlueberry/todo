import { Component } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthFormLayoutComponent } from '@shared/ui/auth-form-layout/auth-form-layout';
import { passwordMatchValidator } from './password-match.validator';

@Component({
  selector: 'feature-register-by-username',
  templateUrl: './register-by-username.html',
  imports: [ReactiveFormsModule, AuthFormLayoutComponent],
})
export class RegisterByUsername {
  protected registerForm = new FormGroup({
    login: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(4), Validators.maxLength(25)]
    }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8)] }),
    confirmPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  }, { validators: [passwordMatchValidator] });

  protected register(): void {
    alert('Attempted registration with: ' + JSON.stringify(this.registerForm.value));
  }
}
