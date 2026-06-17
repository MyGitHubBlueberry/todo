import { Component, input, output, OnInit } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { CategoryResponseDto } from "@entities/category/api/types";
import { TaskCreateDto, TaskResponseDto, TaskUpdateDto } from "@entities/task/api/types";
import { TaskCategoriesComponent } from "@features/task-categories/task-categories.component";
import { ModalLayoutComponent } from "@shared/ui/window-layout/modal-layout.component";

@Component({
  selector: 'app-task-form',
  templateUrl: './task-form.html',
  imports: [FormsModule, TaskCategoriesComponent, ModalLayoutComponent]
})
export class TaskFormComponent implements OnInit {
  public readonly inputTask = input<TaskResponseDto | null>(null);
  public readonly categories = input.required<CategoryResponseDto[]>();

  public readonly onClose = output<void>();
  public readonly onCreate = output<TaskCreateDto>();
  public readonly onUpdate = output<TaskUpdateDto>();

  protected formData!: TaskUpdateDto;
  protected isEditMode = false;

  ngOnInit() {
    const currentTask = this.inputTask();
    this.isEditMode = currentTask !== null;

    if (currentTask) {
      this.formData = {
        title: currentTask.title,
        body: currentTask.body,
        categoryIds: currentTask.categories
          ? currentTask.categories.map(c => c.id)
          : [],
      };
    } else {
      this.formData = {
        title: '',
        body: '',
        categoryIds: [],
      };
    }
  }

  protected submitForm() {
    if (this.isEditMode) {
      this.onUpdate.emit(this.formData);
    } else {
      this.onCreate.emit(this.formData as TaskCreateDto);
    }
  }
}
