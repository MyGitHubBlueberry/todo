import { Component, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryResponseDto } from '@entities/category/api/types';
import { SortBy, TaskStatus } from '@entities/task/api/types';
import { TaskCategoriesComponent } from '@features/task-categories/task-categories.component';

@Component({
  selector: 'app-task-filter-bar',
  templateUrl: './task-filter-bar.html',
  imports: [FormsModule, TaskCategoriesComponent],
})
export class TaskFilterBarComponent {
  public categories = input.required<CategoryResponseDto[]>();

  public selectedCategoryIds = model<number[] | null>(null);
  public selectedStatus = model<TaskStatus | null>(null);
  public sortBy = model<SortBy>();

  public onSearch = output<string>();
}
