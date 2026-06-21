import { Component, input, output } from "@angular/core";
import { CategoryResponseDto } from "@entities/category/api/types";
import { TaskCategoriesComponent } from "@features/task-categories/task-categories.component";
import { ModalLayoutComponent } from "@shared/ui/window-layout/modal-layout.component";

@Component({
  selector: 'app-delete-task-categories',
  templateUrl: './delete-task-categories.html',
  imports: [ModalLayoutComponent, TaskCategoriesComponent]
})
export class DeleteTaskCategoriesComponent {
  public readonly categories = input.required<CategoryResponseDto[]>();
  public onDeleteConfirmed = output<number[]>();
  public onClose = output<void>();

  protected isDeleteCategoryConfirmationMenuOpen = false;
  protected categoriesToDeleteById: number[] = [];
}
