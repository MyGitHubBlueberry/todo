import { DatePipe } from '@angular/common';
import { Component, input, output, signal } from '@angular/core';
import { TaskResponseDto } from '@entities/task/api/types';

@Component({
  selector: 'app-task-card',
  templateUrl: './task-card.html',
  imports: [DatePipe],
})
export class TaskCardComponent {
  public task = input.required<TaskResponseDto>();
  public onToggle = output<TaskResponseDto>();
  public onEdit = output<TaskResponseDto>();
  protected readonly isExpanded = signal(false);

  toggleExpand() {
    this.isExpanded.update(val => !val);
  }
}
