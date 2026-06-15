import { Component, input, output } from '@angular/core';
import { ReactiveFormsModule, FormGroup } from '@angular/forms';

@Component({
  selector: 'shared-auth-form-layout',
  templateUrl: './auth-form-layout.html',
  styleUrl: './auth-form-layout.css',
  imports: [ReactiveFormsModule],
})
export class AuthFormLayoutComponent {
  readonly formGroup = input.required<FormGroup>();
  readonly submitButtonText = input<string>('Submit');
  readonly backendError = input<string | null>(null);

  readonly formSubmit = output<void>();

  protected onSubmit(): void {
    this.formSubmit.emit();
  }
}
