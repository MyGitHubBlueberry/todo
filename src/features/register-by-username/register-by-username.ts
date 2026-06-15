import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';
import { AuthFormLayoutComponent } from '@shared/ui/auth-form-layout/auth-form-layout';
import { SessionService } from '@entities/session/model/session.service';
import { Router } from '@angular/router';
import { RegistrationDto } from '@entities/session/api/types';
import { passwordMatchValidator } from './password-match.validator';

@Component({
  selector: 'feature-register-by-username',
  templateUrl: './register-by-username.html',
  imports: [ReactiveFormsModule, AuthFormLayoutComponent],
})
export class RegisterByUsername {
  private readonly sessionService = inject(SessionService);
  private readonly router = inject(Router);

  private readonly isLoading = signal(false);
  protected readonly backendError = signal<string | null>(null);

  protected registerForm = new FormGroup({
    login: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(4), Validators.maxLength(25)]
    }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(8)] }),
    confirmPassword: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  }, { validators: [passwordMatchValidator] });

  protected register(): void {
    if (this.registerForm.invalid) return;

    this.isLoading.set(true);
    this.backendError.set(null);

    const payload: RegistrationDto = {
      login: this.registerForm.value.login!,
      password: this.registerForm.value.password!,
    };

    this.sessionService.register(payload).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/'])
      },
      error: err => {
        this.isLoading.set(false);
        this.backendError.set(err.error?.message || 'An unexpected error occurred.');
      },
    });
  }
}
