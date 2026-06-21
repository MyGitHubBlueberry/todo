import { Component, input, output } from "@angular/core";
import { TaskResponseDto } from "@entities/task/api/types";
import { ModalLayoutComponent } from "@shared/ui/window-layout/modal-layout.component";

@Component({
  selector: 'app-delete-task-confirmation',
  templateUrl: './delete-task-confirmation.html',
  imports: [ModalLayoutComponent]
})
export class DeleteTaskConfirmationComponent {
  // Accepts the specific task object
  public readonly task = input.required<TaskResponseDto>();

  // Emits the ID back to the parent
  public readonly onConfirmDelete = output<number>();
  public readonly onClose = output<void>();
}
