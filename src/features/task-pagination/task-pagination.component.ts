import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-task-pagination',
  template: `
    <footer class="pagination">
      <button [disabled]="currentPage() === 0 || isLoading()" (click)="onPrevious.emit()">
        Previous
      </button>

      <span>Page {{ currentPage() + 1 }}</span>

      <button [disabled]="tasksCount() < pageSize() || isLoading()" (click)="onNext.emit()">
        Next
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
