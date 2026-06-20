import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-task-pagination',
  template: `
    <footer class="flex items-center justify-center">
      <button [disabled]="currentPage() === 0 || isLoading()" (click)="onPrevious.emit()">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="24" height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="lucide lucide-chevron-left-icon lucide-chevron-left"
          class="stroke-brand-border hover:stroke-brand-muted transition-colors"
        >
          <path d="m15 18-6-6 6-6"/>
        </svg>
      </button>

      <span class="flex items-center justify-center w-7 h-7 font-semibold bg-brand-primary rounded-full text-white">
        {{ currentPage() + 1 }}
      </span>

      <button [disabled]="tasksCount() < pageSize() || isLoading()" (click)="onNext.emit()">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="24" height="24"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="lucide lucide-chevron-right-icon lucide-chevron-right"
          class="stroke-brand-border hover:stroke-brand-muted transition-colors"
        >
          <path d="m9 18 6-6-6-6"/>
        </svg>
      </button>
    </footer>
  `
})
export class TaskPaginationComponent {
  public currentPage = input.required<number>();
  public pageSize = input.required<number>();
  public tasksCount = input.required<number>();
  public isLoading = input.required<boolean>();

  public onNext = output<void>();
  public onPrevious = output<void>();
}
