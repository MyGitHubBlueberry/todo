import { Component, input, model } from "@angular/core";
import { CategoryResponseDto } from "@entities/category/api/types";

@Component({
  selector: 'app-task-categories',
  template: `
      <select multiple class="search-categories" [(ngModel)]="selectedCategoryIds">
        @for (category of categories(); track category.id) {
          <option [ngValue]="category.id" class="search-category">
            {{ category.name }}
          </option>
        }
      </select>
  `
})
export class TaskCategoriesComponent {
  public categories = input.required<CategoryResponseDto[]>();
  public selectedCategoryIds = model<number[]>([]);
}
