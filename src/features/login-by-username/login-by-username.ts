import { Component } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthFormLayoutComponent } from '@shared/ui/auth-form-layout/auth-form-layout';

@Component({
  selector: 'feature-login-by-username',
  templateUrl: './login-by-username.html',
  imports: [ReactiveFormsModule, AuthFormLayoutComponent],
})
export class LoginByUsername {
  protected readonly loginForm = new FormGroup({
    login: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required]
    }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  protected login(): void {
    alert('Attempted login with: ' + JSON.stringify(this.loginForm.value));
  }
}
