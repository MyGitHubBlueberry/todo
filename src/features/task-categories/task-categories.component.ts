import { Component, input, model } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { CategoryResponseDto } from "@entities/category/api/types";

@Component({
  selector: 'app-task-categories',
  imports: [FormsModule],
  template: `
      <select
        multiple
        class="search-categories"
        [(ngModel)]="selectedCategories"
        [compareWith]="compareById"
      >
        @for (category of categories(); track category.id) {
          <option [ngValue]="category" class="search-category">
            {{ category.name }}
          </option>
        }
      </select>
  `
})
export class TaskCategoriesComponent {
  public categories = input.required<CategoryResponseDto[]>();
  public selectedCategories = model<CategoryResponseDto[]>([]);

  protected compareById(c1: CategoryResponseDto, c2: CategoryResponseDto): boolean {
    return c1 && c2 ? c1.id === c2.id : c1 === c2;
  }
}
