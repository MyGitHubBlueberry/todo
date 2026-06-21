import { Component, input } from "@angular/core";

@Component({
  selector: 'app-toast',
  styleUrl: './toast.component.css',
  template: `
  <div class="container w-auto p-0 fixed m-4 top-0 right-0 md:m-8 shadow-lg overflow-hidden min-w-64 z-50">

    <div class="p-4">
      <p class="text-brand-text">{{ message() }}</p>
    </div>

    <div
      class="h-1 bg-brand-primary animate-shrink-bar origin-left"
      [style.animation-duration.s]="seconds()"
    ></div>

  </div>
  `,
})
export class ToastComponent {
  public readonly message = input.required<string>();
  public readonly seconds = input.required<number>();
}
