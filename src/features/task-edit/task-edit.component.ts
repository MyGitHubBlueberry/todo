import { Component, input, model, output, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { CategoryResponseDto } from "@entities/category/api/types";
import { TaskResponseDto, TaskUpdateDto } from "@entities/task/api/types";
import { TaskCategoriesComponent } from "@features/task-categories/task-categories.component";
import { ModalLayoutComponent } from "@shared/ui/window-layout/modal-layout.component";

@Component({
  selector: 'app-task-edit',
  templateUrl: './task-edit.html',
  imports: [FormsModule, TaskCategoriesComponent, ModalLayoutComponent]
})
export class TaskEditComponent {
  public readonly task = input.required<TaskResponseDto>();
  public readonly categories = input.required<CategoryResponseDto[]>();

  public readonly onSave = output<TaskUpdateDto>();
  public readonly onClose = output<void>();

  protected updatedTask!: TaskUpdateDto;

  ngOnInit() {
    const currentTask = this.task();
    this.updatedTask = {
      title: currentTask.title,
      body: currentTask.body,
      categoryIds: currentTask.categories
        ? [...currentTask.categories.map(c => c.id)]
        : [],
    };
  }
}
