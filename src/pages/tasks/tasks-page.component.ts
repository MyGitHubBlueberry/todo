import { Component, inject, OnInit, signal } from "@angular/core";
import { Router } from "@angular/router";
import { CategoryResponseDto } from "@entities/category/api/types";
import { SessionService } from "@entities/session/model/session.service";
import { TaskApiService } from "@entities/task/api/task-api.service";
import { TaskResponseDto, TaskGetQueryDto, TaskStatus, TaskUpdateDto, TaskCreateDto } from "@entities/task/api/types";
import { TaskCardComponent } from "@entities/task/ui/task-card";
import { TaskFilterBarComponent } from "@features/task-filters/task-filter-bar.component";
import { TaskFormComponent } from "@features/task-form/task-form.component";
import { TaskPaginationComponent } from "@features/task-pagination/task-pagination.component";

@Component({
  selector: 'app-tasks-page',
  templateUrl: './tasks-page.html',
  styleUrl: './tasks-page.css',
  imports: [TaskFilterBarComponent, TaskPaginationComponent, TaskCardComponent, TaskFormComponent],
})
export class TaskPageComponent implements OnInit {
  private readonly api = inject(TaskApiService);
  private readonly temp_sessionApi = inject(SessionService);
  private readonly temp_router = inject(Router);

  protected readonly tasks = signal<TaskResponseDto[]>([]);
  protected readonly categories = signal<CategoryResponseDto[]>([]);
  protected readonly taskToEdit = signal<TaskResponseDto | null>(null);
  protected readonly isCreateMenuOpen = signal<boolean>(false);

  protected readonly searchTerm = signal<string | null>(null);
  protected readonly selectedCategoryIds = signal<number[] | null>(null);
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

    this.api.patchStatus(task.id, {
      status: newStatus,
    }).subscribe({
      error: () => {
        this.tasks.update(currentTasks =>
          currentTasks.map(t => t.id === task.id ? { ...t, status: task.status } : t)
        );
      },
    });
  }

  protected openTaskCreateMenu() {
    this.taskToEdit.set(null);
    this.isCreateMenuOpen.set(true);
  }

  protected openTaskEditMenu(task: TaskResponseDto) {
    this.isCreateMenuOpen.set(false);
    this.taskToEdit.set(task);
  }

  protected closeMenus() {
    this.taskToEdit.set(null);
    this.isCreateMenuOpen.set(false);
  }

  protected createTask(dto: TaskCreateDto) {
    this.api.create(dto).subscribe({
      next: (newTask) => {
        this.tasks.update(currentTasks => [newTask, ...currentTasks]);
        this.closeMenus();
      },
      error: (err) => {
        console.error('Failed to create task', err);
        // todo: show error popup
      }
    });
  }

  protected updateTask(id: number, dto: TaskUpdateDto) {
    const previousTasks = this.tasks();

    this.tasks.update(currentTasks =>
      currentTasks.map(t => t.id === id ? { ...t, ...dto } : t)
    );

    this.closeMenus();

    this.api.put(id, dto).subscribe({
      error: (err) => {
        //todo: show it as popup message
        console.error('Failed to update task', err);
        this.tasks.set(previousTasks);
      }
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

  protected logout() {
    this.temp_sessionApi.logout();
    this.temp_router.navigate(['/auth']);
  }
}
