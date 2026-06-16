import { Component, computed, inject, OnInit, signal } from "@angular/core";
import { CategoryResponseDto } from "@entities/category/api/types";
import { TaskApiService } from "@entities/task/api/task-api.service";
import { TaskResponseDto, TaskGetQueryDto, TaskStatus } from "@entities/task/api/types";
import { TaskCardComponent } from "@entities/task/ui/task-card";
import { TaskFilterBarComponent } from "@features/task-filters/task-filter-bar.component";
import { TaskPaginationComponent } from "@features/task-pagination/task-pagination.component";

@Component({
  selector: 'app-tasks-page',
  templateUrl: './tasks-page.html',
  styleUrl: './tasks-page.css',
  imports: [TaskFilterBarComponent, TaskPaginationComponent, TaskCardComponent],
})
export class TaskPageComponent {
  private readonly api = inject(TaskApiService);

  protected readonly tasks = signal<TaskResponseDto[]>([]);
  protected readonly categories = signal<CategoryResponseDto[]>([]);

  protected readonly searchTerm = signal<string | null>(null);
  protected readonly selectedCategoryIds = signal<number[]>([]);
  protected readonly selectedStatus = signal<TaskStatus | null>(null);

  protected readonly pageSize = signal<number>(10);
  protected readonly currentPage = signal<number>(0);
  protected readonly totalCount = signal<number>(0);

  protected readonly isLoading = signal<boolean>(false);

  ngOnInit() {
    this.fetchTasks();
    //todo: this.fetchCategories();
  }

  protected toggleTask(task: TaskResponseDto) {
    const newStatus: TaskStatus = task.status == 'Done' ? 'Pending' : 'Done';

    this.tasks.update(currentTasks =>
      currentTasks.map(t => t.id === task.id ? { ...t, status: newStatus } : t)
    );

    this.api.putStatus(task.id, {
      status: newStatus,
    }).subscribe({
      error: () => {
        this.tasks.update(currentTasks =>
          currentTasks.map(t => t.id === task.id ? { ...t, status: task.status } : t)
        );
      },
    });
  }

  protected deleteTask(id: number) {
    const previousTasks = this.tasks();
    this.tasks.set(previousTasks.filter(t => t.id !== id));

    this.api.delete(id).subscribe({
      error: () => {
        this.tasks.set(previousTasks);
      }
    })
  }

  //todo: updateTask
  //todo: addTask

  protected nextPage() {
    if (this.tasks().length < this.pageSize()) return;
    this.currentPage.update(p => p + 1);
    this.fetchTasks();
  }

  protected previousPage() {
    if (this.currentPage() == 0) return;
    this.currentPage.update(p => p - 1);
    this.fetchTasks();
  }

  protected fetchTasks() {
    this.isLoading.set(true);
    var query: TaskGetQueryDto = {
      page: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm(),
      categoryIds: this.selectedCategoryIds(),
      selectedStatus: this.selectedStatus()
    };
    this.api.get(query).subscribe({
      next: (data) => {
        this.isLoading.set(false);
        this.tasks.set(data.tasks);
        this.totalCount.set(data.totalCount);
      },
      error: () => {
        this.isLoading.set(false);
        //todo: show some error?
      }
    });
  }
}
