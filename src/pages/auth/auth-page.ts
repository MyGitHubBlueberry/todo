import { Component, signal } from "@angular/core";
import { LoginByUsername } from "@features/login-by-username/login-by-username";
import { RegisterByUsername } from "@features/register-by-username/register-by-username";

@Component({
  selector: 'page-auth',
  templateUrl: './auth-page.html',
  styleUrl: './auth-page.css',
  imports: [LoginByUsername, RegisterByUsername],
})
export class AuthPage {
  protected currentForm = signal<'login' | 'register'>('login');

  protected showLoginForm() {
    this.currentForm.set('login');
  }

  protected showRegisterForm() {
    this.currentForm.set('register');
  }
}
