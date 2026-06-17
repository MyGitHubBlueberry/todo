import { Component, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryResponseDto } from '@entities/category/api/types';
import { TaskStatus } from '@entities/task/api/types';
import { TaskCategoriesComponent } from '@features/task-categories/task-categories.component';

@Component({
  selector: 'app-task-filter-bar',
  imports: [FormsModule, TaskCategoriesComponent],
  template: `
    <div class="search-controls">
      <app-task-categories
        [categories]="categories()"
        [(selectedCategoryIds)]="selectedCategoryIds"
      />

      <select class="search-filters" [(ngModel)]="selectedStatus">
        <option [ngValue]="null" class="search-filter">All</option>
        <option value="Done" class="search-filter">Done</option>
        <option value="Pending" class="search-filter">Pending</option>
      </select>

      <input type="text" placeholder="Search tasks..." #searchInput
        (keyup.enter)="onSearch.emit(searchInput.value)" />

      <button (click)="onSearch.emit(searchInput.value)">Search</button>
    </div>
    `
})
export class TaskFilterBarComponent {
  public categories = input.required<CategoryResponseDto[]>();

  public selectedCategoryIds = model<number[] | null>(null);
  public selectedStatus = model<TaskStatus | null>(null);

  public onSearch = output<string>();
}
