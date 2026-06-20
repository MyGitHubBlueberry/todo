import { Component, input, model } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { CategoryResponseDto } from "@entities/category/api/types";

@Component({
  selector: 'app-task-categories',
  imports: [FormsModule],
  template: `
    <div class="flex flex-col border border-brand-border rounded-md bg-brand-dark h-min max-h-40 overflow-y-auto custom-scrollbar">
      @for (category of categories(); track category.id) {
        <label class="flex items-center px-3 py-2 cursor-pointer hover:bg-brand-surface transition-colors group">
          <input
            type="checkbox"
            class="custom-checkbox mr-3"
            [checked]="selectedCategoryIds().includes(category.id)"
            (change)="toggleCategory(category.id)"
          />
          <span class="text-brand-text group-hover:text-white transition-colors">
            {{ category.name }}
          </span>
        </label>
      }

      @empty {
        <p class="text-brand-muted text-sm text-center py-4">No categories available.</p>
      }
    </div>
  `
})
export class TaskCategoriesComponent {
  public readonly categories = input.required<CategoryResponseDto[]>();
  public readonly selectedCategoryIds = model<number[]>([]);

  protected toggleCategory(categoryId: number): void {
    const currentIds = this.selectedCategoryIds();

    if (currentIds.includes(categoryId)) {
      this.selectedCategoryIds.set(currentIds.filter(id => id !== categoryId));
    } else {
      this.selectedCategoryIds.set([...currentIds, categoryId]);
    }
  }
}
