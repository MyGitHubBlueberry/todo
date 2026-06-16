import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { TaskCreateDto, TaskGetQueryDto, TaskResponseDto, TaskStatusUpdateDto, TaskUpdateDto } from "./types";

@Injectable({ providedIn: 'root' })
export class TaskApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api/tasks';

  public create(dto: TaskCreateDto): Observable<TaskResponseDto> {
    return this.http.post<TaskResponseDto>(`${this.baseUrl}/create`, dto);
  }

  public delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/delete/${id}`);
  }

  public getById(id: number): Observable<TaskResponseDto> {
    return this.http.get<TaskResponseDto>(`${this.baseUrl}/get/${id}`);
  }

  public get(queryDto?: TaskGetQueryDto): Observable<TaskResponseDto[]> {
    let queryParams = new HttpParams();

    if (queryDto) {
      Object.entries(queryDto).forEach(([key, value]) => {
        queryParams = queryParams.append(key, value);
      });
    }

    return this.http.get<TaskResponseDto[]>(`${this.baseUrl}/get`, { params: queryParams });
  }

  public put(id: number, dto: TaskUpdateDto): Observable<TaskResponseDto> {
    return this.http.put<TaskResponseDto>(`${this.baseUrl}/put/${id}`, dto);
  }

  public putStatus(id: number, dto: TaskStatusUpdateDto): Observable<TaskResponseDto> {
    return this.http.put<TaskResponseDto>(`${this.baseUrl}/put/status/${id}`, dto);
  }
}
