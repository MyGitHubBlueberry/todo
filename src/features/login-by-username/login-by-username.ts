import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginDto } from '@entities/session/api/types';
import { SessionService } from '@entities/session/model/session.service';
import { AuthFormLayoutComponent } from '@shared/ui/auth-form-layout/auth-form-layout';

@Component({
  selector: 'feature-login-by-username',
  templateUrl: './login-by-username.html',
  imports: [ReactiveFormsModule, AuthFormLayoutComponent],
})
export class LoginByUsername {
  private readonly router = inject(Router);
  private readonly sessionService = inject(SessionService);
  private readonly isLoading = signal(false);

  protected readonly backendError = signal<string | null>(null);

  protected readonly loginForm = new FormGroup({
    login: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required]
    }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  protected login(): void {
    if (this.loginForm.invalid) return;

    this.isLoading.set(true);
    this.backendError.set(null);

    var payload: LoginDto = {
      login: this.loginForm.value.login!,
      password: this.loginForm.value.password!,
    };
    this.sessionService.login(payload).subscribe({
      next: _ => {
        this.isLoading.set(false);
        this.router.navigate(['/auth']);
      },
      error: err => {
        this.isLoading.set(false);
        this.backendError.set(err.error?.message || 'An unexpected error occurred.');
      }
    });
  }
}
