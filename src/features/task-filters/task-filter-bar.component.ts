import { Component, input, model, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CategoryResponseDto } from '@entities/category/api/types';
import { SortBy, TaskStatus } from '@entities/task/api/types';

@Component({
  selector: 'app-task-filter-bar',
  templateUrl: './task-filter-bar.html',
  imports: [ FormsModule ],
})
export class TaskFilterBarComponent {
  public categories = input.required<CategoryResponseDto[]>();
  public selectedCategoryIds = model.required<number[]>();
  public sortBy = model.required<SortBy>();

  public selectedStatus = model<TaskStatus | null>(null);

  public showCreateCategoryForm = output<void>();
  public showDeleteCategoryForm = output<void>();
  public onSearch = output<string>();

  protected onCheckboxChange(categoryId: number, event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;

    this.selectedCategoryIds.update(currentIds => {
      if (isChecked) {
        return [...currentIds, categoryId];
      } else {
        return currentIds.filter(id => id !== categoryId);
      }
    });
  }
}
