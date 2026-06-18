import { Component, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryResponseDto } from '@entities/category/api/types';
import { SortBy, TaskStatus } from '@entities/task/api/types';
import { FilterAddCategoriesComponent } from '@features/filter-add-categories/filter-add-categories.component';

@Component({
  selector: 'app-task-filter-bar',
  templateUrl: './task-filter-bar.html',
  imports: [
    FormsModule,
    FilterAddCategoriesComponent
  ],
})
export class TaskFilterBarComponent {
  public categories = input.required<CategoryResponseDto[]>();
  public selectedCategories = model.required<CategoryResponseDto[]>();
  public sortBy = model.required<SortBy>();

  public selectedStatus = model<TaskStatus | null>(null);

  public onCreateCategory = output<string>();
  public onSearch = output<string>();

  protected showAddFilterCategories = false;

  protected removeFilterCategory(id: number) {
    this.selectedCategories.update(current =>
      current.filter(c => c.id !== id)
    );
  }
}
