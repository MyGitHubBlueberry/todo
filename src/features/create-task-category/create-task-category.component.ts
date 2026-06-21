import { Component, output } from "@angular/core";
import { ModalLayoutComponent } from "@shared/ui/window-layout/modal-layout.component";

@Component({
  selector: 'app-create-task-category',
  imports: [ModalLayoutComponent],
  template: `
    <app-modal-layout
      title="Create Task Category"
      (onClose)="onClose.emit()"
    >
      <label>
        Name
        <input type="text" #categoryName (input)="0" class="form-input mt-1 w-full" placeholder="e.g. Work, Personal" autofocus />
      </label>

      <button
        modal-footer
        class="btn-primary"
        [disabled]="!categoryName.value || categoryName.value.trim() === ''"
        (click)="onCreate.emit(categoryName.value)"
      >
        Create
      </button>
    </app-modal-layout>
  `
})
export class CreateTaskCategoryComponent {
  public readonly onClose = output<void>();
  public readonly onCreate = output<string>();
}
