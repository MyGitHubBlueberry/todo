import { Component, computed, inject, OnInit, signal } from "@angular/core";
import { CategoryResponseDto } from "@entities/category/api/types";
import { TaskApiService } from "@entities/task/api/task-api.service";
import { TaskResponseDto, TaskGetQueryDto, TaskStatus } from "@entities/task/api/types";

@Component({
  selector: 'page-tasks',
  templateUrl: './tasks-page.html',
  styleUrl: './tasks-page.css',
})
export class TaskPage {
  private readonly api = inject(TaskApiService);
  protected readonly tasks = signal<TaskResponseDto[]>([]);
  private readonly tasksMap =
    computed(() => new Map(this.tasks().map(t => [t.id, t] as const)));
  protected readonly categories = signal<CategoryResponseDto[]>([]);
  protected readonly searchTerm = signal<string | null>(null);
  protected readonly currentPage = signal<number>(0);
  protected readonly pageSize = signal<number>(10);
  protected readonly selectedCategoryIds = signal<number[]>([]);
  protected readonly selectedStatus = signal<TaskStatus | null>(null);
  protected readonly totalCount = signal<number>(0);

  protected readonly isLoading = signal<boolean>(false);

  ngOnInit() {
    this.fetchTasks();
  }

  protected toggleTask(task: TaskResponseDto) {
    task.status = task.status == 'Done' ? 'Pending' : 'Done';
    this.api.putStatus(task.id, {
      status: task.status,
    });
    //todo: manage subscribtion?
  }

  protected deleteTask(id: number) {
    if (!this.tasksMap().has(id)) return;

    this.api.delete(id).subscribe({
      next: () => {
        if (this.tasksMap().has(id)) {
          this.tasks.update(t => t.filter(t => t.id != id));
        }
      },
      // error (todo)
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
      error: (err) => {
        this.isLoading.set(false);
        //todo: show some error?
      }
    });
  }
}
