import { SlicePipe } from "@angular/common";
import { Component, inject, OnInit, signal } from "@angular/core";
import { Router } from "@angular/router";
import { CategoryApiService } from "@entities/category/api/category.service";
import { CategoryResponseDto } from "@entities/category/api/types";
import { SessionService } from "@entities/session/model/session.service";
import { TaskApiService } from "@entities/task/api/task-api.service";
import { TaskResponseDto, TaskGetQueryDto, TaskStatus, TaskUpdateDto, TaskCreateDto, SortBy } from "@entities/task/api/types";
import { TaskCardComponent } from "@entities/task/ui/task-card";
import { CreateTaskCategoryComponent } from "@features/create-task-category/create-task-category.component";
import { DeleteTaskCategoriesComponent } from "@features/delete-task-categories/delete-task-categories.component";
import { DeleteTaskConfirmationComponent } from "@features/delete-task-confirmation/delete-task-confirmation.components";
import { TaskFilterBarComponent } from "@features/task-filters/task-filter-bar.component";
import { TaskFormComponent } from "@features/task-form/task-form.component";
import { TaskPaginationComponent } from "@features/task-pagination/task-pagination.component";
import { ToastComponent } from "@shared/ui/toast/toast.component";
import { ToastService } from "@shared/ui/toast/toast.service";

@Component({
  selector: 'app-tasks-page',
  templateUrl: './tasks-page.html',
  styleUrl: './tasks-page.css',
  imports: [
    SlicePipe,
    TaskFilterBarComponent,
    TaskPaginationComponent,
    TaskCardComponent,
    TaskFormComponent,
    DeleteTaskCategoriesComponent,
    DeleteTaskConfirmationComponent,
    CreateTaskCategoryComponent,
    ToastComponent
  ],
})
export class TaskPageComponent implements OnInit {
  private readonly taskApi = inject(TaskApiService);
  private readonly categoryApi = inject(CategoryApiService);
  private readonly sessionApi = inject(SessionService);
  private readonly router = inject(Router);

  protected readonly toastService = inject(ToastService);
  protected readonly tasks = signal<TaskResponseDto[]>([]);
  protected readonly categories = signal<CategoryResponseDto[]>([]);
  protected readonly taskToEdit = signal<TaskResponseDto | null>(null);
  protected readonly taskToDelete = signal<TaskResponseDto | null>(null);
  protected readonly categoriesToDeleteById = signal<number[]>([])

  protected readonly isCreateMenuOpen = signal<boolean>(false);
  protected readonly isCreateCategoryMenuOpen = signal<boolean>(false);
  protected readonly isDeleteCategoryMenuOpen = signal<boolean>(false);
  protected readonly isDeleteCategoryConfirmationMenuOpen = signal<boolean>(false);

  protected readonly selectedCategoryIds = signal<number[]>([]);
  protected readonly sortBy = signal<SortBy>("CrtAsc");
  protected readonly searchTerm = signal<string | null>(null);
  protected readonly selectedStatus = signal<TaskStatus | null>(null);

  protected readonly pageSize = signal<number>(10);
  protected readonly currentPage = signal<number>(0);
  protected readonly totalCount = signal<number>(0);

  protected readonly isLoading = signal<boolean>(false);

  ngOnInit() {
    this.fetchTasks();
    this.fetchCategories();
  }

  protected sortTaskPage() {
    this.tasks.update(tasks => {
      const sortedTasks = [...tasks];

      sortedTasks.sort((a, b) => {
        switch (this.sortBy()) {
          case 'AlphAsc':
            return a.title.localeCompare(b.title);
          case 'AlphDsc':
            return b.title.localeCompare(a.title);
          case 'CrtAsc':
            return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
          case 'CrtDsc':
            return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
          case 'UpdAsc':
            if (!a.updatedAt && !b.updatedAt) return 0;
            if (!a.updatedAt) return 1;
            if (!b.updatedAt) return -1;
            return new Date(a.updatedAt).getTime() - new Date(b.updatedAt).getTime();
          case 'UpdDsc':
            if (!a.updatedAt && !b.updatedAt) return 0;
            if (!a.updatedAt) return 1;
            if (!b.updatedAt) return -1;
            return new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime();
          default:
            return 0;
        }
      });

      return sortedTasks;
    });
  }

  protected toggleTask(task: TaskResponseDto) {
    const newStatus: TaskStatus = task.status == 'Done' ? 'Pending' : 'Done';

    this.tasks.update(currentTasks =>
      currentTasks.map(t => t.id === task.id ? { ...t, status: newStatus } : t)
    );

    this.taskApi.patchStatus(task.id, {
      status: newStatus,
    }).subscribe({
      error: () => {
        this.tasks.update(currentTasks =>
          currentTasks.map(t => t.id === task.id ? { ...t, status: task.status } : t)
        );
      },
    });
  }

  protected closeMenus() {
    this.taskToEdit.set(null);
    this.taskToDelete.set(null);
    this.isCreateMenuOpen.set(false);
    this.isCreateCategoryMenuOpen.set(false);
    this.isDeleteCategoryMenuOpen.set(false);
    this.isDeleteCategoryConfirmationMenuOpen.set(false);
  }

  protected createTask(dto: TaskCreateDto) {
    this.taskApi.create(dto).subscribe({
      next: (newTask) => {
        this.tasks.update(currentTasks => [newTask, ...currentTasks]);
        this.closeMenus();
      },
      error: (err) => {
        const serverMessage = err.error?.message || 'Failed to create task due to a server error.';
        this.toastService.showError(serverMessage, 5);
      }
    });
  }

  protected updateTask(id: number, dto: TaskUpdateDto) {
    const previousTasks = this.tasks();

    const updatedCategories = this.categories().filter(c =>
      dto.categoryIds.includes(c.id)
    );

    this.tasks.update(currentTasks =>
      currentTasks.map(t =>
        t.id === id
          ? { ...t, ...dto, categories: updatedCategories }
          : t
      )
    );

    this.closeMenus();

    this.taskApi.put(id, dto).subscribe({
      next: (updatedTaskFromServer) => {
        this.tasks.update(tasks => tasks.map(t => t.id === id ? updatedTaskFromServer : t));
      },
      error: (err) => {
        const serverMessage = err.error?.message || 'Failed to update task due to a server error.';
        this.toastService.showError(serverMessage, 5);
        this.tasks.set(previousTasks);
      }
    });
  }

  protected deleteTask(id: number) {
    const previousTasks = this.tasks();
    this.tasks.set(previousTasks.filter(t => t.id !== id));

    this.closeMenus();

    this.taskApi.delete(id).subscribe({
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

  protected fetchCategories() {
    this.categoryApi.get().subscribe({
      next: (data) => {
        this.categories.set(data);
      }
    });
  }

  protected createCategory(name: string) {
    this.closeMenus();
    this.categoryApi.create({ name }).subscribe({
      next: (newCategory) => {
        this.categories.update(list => [...list, newCategory]);
      },
      error: (err) => {
        const serverMessage = err.error?.message || 'Failed to create category due to a server error.';
        this.toastService.showError(serverMessage, 5);
      }
    });
  }

  protected deleteCategories(ids: number[]) {
    this.closeMenus();

    const previousTasks = this.tasks();
    const previousCategories = this.categories();

    this.categories.update(list => list.filter(c => !ids.includes(c.id)));

    this.tasks.update(currentTasks =>
      currentTasks.map(task => {
        const hasDeletedCategory = task.categories?.some(c => ids.includes(c.id));

        if (hasDeletedCategory) {
          return {
            ...task,
            categories: task.categories.filter(c => !ids.includes(c.id))
          };
        }

        return task;
      })
    );

    this.categoryApi.delete(ids).subscribe({
      error: (err) => {
        const serverMessage = err.error?.message || 'Failed to delete categories due to a server error.';
        this.toastService.showError(serverMessage, 5);
        this.categories.set(previousCategories);
        this.tasks.set(previousTasks);
      }
    });
  }

  protected fetchTasks() {
    this.isLoading.set(true);
    var query: TaskGetQueryDto = {
      page: this.currentPage(),
      pageSize: this.pageSize(),
      sortBy: this.sortBy(),
      searchTerm: this.searchTerm(),
      categoryIds: this.selectedCategoryIds(),
      selectedStatus: this.selectedStatus()
    };
    this.taskApi.get(query).subscribe({
      next: (data) => {
        this.isLoading.set(false);
        this.tasks.set(data.tasks);
        this.totalCount.set(data.totalCount);
        this.currentPage.set(data.page);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  protected logout() {
    this.sessionApi.logout();
    this.router.navigate(['/auth']);
  }
}
