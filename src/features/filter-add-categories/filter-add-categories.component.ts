import { Component, input, model, output, signal } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { CategoryResponseDto } from "@entities/category/api/types";
import { TaskCategoriesComponent } from "@features/task-categories/task-categories.component";
import { ModalLayoutComponent } from "@shared/ui/window-layout/modal-layout.component";

@Component({
  selector: 'app-filter-add-categories',
  templateUrl: './filter-add-categories.html',
  imports: [
    FormsModule,
    ModalLayoutComponent,
    TaskCategoriesComponent
  ]
})
export class FilterAddCategoriesComponent {
  public readonly categories = input.required<CategoryResponseDto[]>();
  public readonly selectedCategories = model<CategoryResponseDto[]>([]);

  public readonly onClose = output<void>();
  public readonly onCreateCategory = output<string>();

  protected newCategoryName = signal<string>('');
  protected isCategoryCreateForm = false;

  protected handleSave() {
    if (this.isCategoryCreateForm) {
      this.onCreateCategory.emit(this.newCategoryName());
      this.newCategoryName.set('');
      this.isCategoryCreateForm = false;
    } else {
      this.onClose.emit();
    }
  }
}
