import { Component, input, output } from '@angular/core';
import { TaskResponseDto } from '@entities/task/api/types';

@Component({
  selector: 'task-card',
  template: `
    <li class="task-card">
      <label>
        <input
          type="checkbox"
          [checked]="task().status === 'Done'"
          (change)="onToggle.emit(task())"
        />
        <span [class.completed]="task().status === 'Done'">
          {{ task().title }}
        </span>
      </label>

      <button class="delete-btn" (click)="onDelete.emit(task().id)">Delete</button>
    </li>
  `
})
export class TaskCard {
  public task = input.required<TaskResponseDto>();
  public onToggle = output<TaskResponseDto>();
  public onDelete = output<number>();
}
