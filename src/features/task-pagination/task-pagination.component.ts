import { Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-task-pagination',
  templateUrl: './task-pagination.html',
})
export class TaskPaginationComponent {
  public currentPage = input.required<number>();
  public pageSize = input.required<number>();
  public tasksCount = input.required<number>();
  public totalTasks = input.required<number>();
  public isLoading = input.required<boolean>();

  public onNext = output<void>();
  public onPrevious = output<void>();
  protected readonly currentPageConverted = computed(() => this.currentPage() + 1);

  protected isLastPage() {
    return this.tasksCount() < this.pageSize() || this.pageSize() * this.currentPageConverted() === this.totalTasks();
  }
}
