import { computed, inject, Injectable, signal } from "@angular/core";
import { TaskApiService } from "../api/task-api.service";
import { TaskCreateDto, TaskResponseDto, TaskStatusUpdateDto, TaskUpdateDto } from "../api/types";
import { Observable, tap, of } from "rxjs";

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly api = inject(TaskApiService);
  public readonly tasks = signal<Map<number, TaskResponseDto>>(new Map());
  public readonly taskArr = computed(() => Array.from(this.tasks().values()));

  public create(dto: TaskCreateDto): Observable<TaskResponseDto> {
    return this.api.create(dto).pipe(
      tap(task => {
        this.tasks.update(map => {
          const newMap = new Map(map);
          newMap.set(task.id, task);
          return newMap;
        });
      })
    );
  }

  public delete(id: number): Observable<void> {
    return this.api.delete(id).pipe(
      tap(() => {
        this.tasks.update(map => {
          const newMap = new Map(map);
          newMap.delete(id);
          return newMap;
        });
      })
    );
  }

  public getById(id: number): Observable<TaskResponseDto> {
    const currentMap = this.tasks();

    if (currentMap.has(id)) {
      return of(currentMap.get(id)!);
    }

    return this.api.getById(id).pipe(
      tap(task => {
        this.tasks.update(map => {
          const newMap = new Map(map);
          newMap.set(task.id, task);
          return newMap;
        });
      })
    );
  }

  public get(): Observable<TaskResponseDto[]> {
    if (this.taskArr().length > 0) {
      return of(this.taskArr()!);
    }

    return this.api.get().pipe(
      tap(tasksArray => {
        this.tasks.update(map => {
          const newMap = new Map(map);
          tasksArray.forEach(t => newMap.set(t.id, t));
          return newMap;
        });
      })
    );
  }

  public put(id: number, dto: TaskUpdateDto): Observable<TaskResponseDto> {
    return this.api.put(id, dto).pipe(
      tap(task => {
        this.tasks.update(map => {
          const newMap = new Map(map);
          newMap.set(id, task);
          return newMap;
        });
      })
    );
  }

  public putStatus(id: number, dto: TaskStatusUpdateDto): Observable<TaskResponseDto> {
    return this.api.putStatus(id, dto).pipe(
      tap(task => {
        this.tasks.update(map => {
          const newMap = new Map(map);
          newMap.set(id, task);
          return newMap;
        });
      })
    );
  }
}
