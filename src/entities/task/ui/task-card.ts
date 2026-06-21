import { DatePipe } from '@angular/common';
import {
  Component,
  input,
  output,
  signal,
  viewChild,
  ElementRef,
  AfterViewInit,
  OnDestroy
} from '@angular/core';
import { TaskResponseDto } from '@entities/task/api/types';

@Component({
  selector: 'app-task-card',
  templateUrl: './task-card.html',
  imports: [DatePipe],
})
export class TaskCardComponent implements AfterViewInit, OnDestroy {
  public task = input.required<TaskResponseDto>();
  public onToggle = output<TaskResponseDto>();
  public onEdit = output<TaskResponseDto>();
  public textContainer = viewChild<ElementRef<HTMLParagraphElement>>('taskBody');

  protected readonly isExpanded = signal(false);
  protected readonly isTextOverflowing = signal(false);

  private resizeObserver?: ResizeObserver;

  ngAfterViewInit() {
    const element = this.textContainer()?.nativeElement;
    if (!element) return;

    this.resizeObserver = new ResizeObserver(() => {
      this.checkOverflow(element);
    });

    this.resizeObserver.observe(element);

    setTimeout(() => this.checkOverflow(element), 0);
  }

  ngOnDestroy() {
    this.resizeObserver?.disconnect();
  }

  private checkOverflow(element: HTMLElement) {
    if (!this.isExpanded()) {
      const hasOverflow = element.scrollHeight > (element.clientHeight + 2);  // +2 helps it work on all browsers
      this.isTextOverflowing.set(hasOverflow);
    }
  }

  toggleExpand() {
    this.isExpanded.update(val => !val);
  }
}
