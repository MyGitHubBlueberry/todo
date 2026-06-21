import { Component, signal } from "@angular/core";
import { LoginByUsernameComponent } from "@features/login-by-username/login-by-username.component";
import { RegisterByUsernameComponent } from "@features/register-by-username/register-by-username.component";

@Component({
  selector: 'app-auth-page',
  templateUrl: './auth-page.html',
  imports: [LoginByUsernameComponent, RegisterByUsernameComponent],
})
export class AuthPageComponent {
  protected currentForm = signal<'login' | 'register'>('login');

  protected showLoginForm() {
    this.currentForm.set('login');
  }

  protected showRegisterForm() {
    this.currentForm.set('register');
  }
}
