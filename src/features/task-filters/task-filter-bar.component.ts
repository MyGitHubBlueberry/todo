import { Component, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryResponseDto } from '@entities/category/api/types';
import { TaskStatus } from '@entities/task/api/types';

@Component({
  selector: 'app-task-filter-bar',
  imports: [FormsModule],
  template: `
    <div class="search-controls">
      <select multiple class="search-categories" [(ngModel)]="selectedCategoryIds">
        @for (category of categories(); track category.id) {
          <option [ngValue]="category.id" class="search-category">
            {{ category.name }}
          </option>
        }
      </select>

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

  public selectedCategoryIds = model<number[]>([]);
  public selectedStatus = model<TaskStatus | null>(null);

  public onSearch = output<string>();
}
