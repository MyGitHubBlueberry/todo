import { Injectable, signal } from '@angular/core';

export type ToastMessage = {
  message: string;
  type: 'error' | 'success';
  durationSeconds: number;
} | null;

@Injectable({ providedIn: 'root' })
export class ToastService {
  public readonly currentToast = signal<ToastMessage>(null);

  private timeoutId?: ReturnType<typeof setTimeout>;

  public showError(message: string, durationSeconds: number = 3) {
    if (this.timeoutId) {
      clearTimeout(this.timeoutId);
    }

    this.currentToast.set({ message, type: 'error', durationSeconds });

    this.timeoutId = setTimeout(() => {
      this.currentToast.set(null);
    }, durationSeconds * 1000);
  }
}
