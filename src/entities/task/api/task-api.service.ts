import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { TaskCreateDto, TaskGetQueryDto, TaskPaginatedResponseDto, TaskResponseDto, TaskStatusUpdateDto, TaskUpdateDto } from "./types";

@Injectable({ providedIn: 'root' })
export class TaskApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = import.meta.env.NG_APP_API_URL + '/tasks';

  public create(dto: TaskCreateDto): Observable<TaskResponseDto> {
    return this.http.post<TaskResponseDto>(this.baseUrl, dto);
  }

  public delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  public getById(id: number): Observable<TaskResponseDto> {
    return this.http.get<TaskResponseDto>(`${this.baseUrl}/${id}`);
  }

  public get(queryDto?: TaskGetQueryDto): Observable<TaskPaginatedResponseDto> {
    let queryParams = new HttpParams();

    if (queryDto) {
      Object.entries(queryDto).forEach(([key, value]) => {
        if (value !== null && value !== undefined) {
          if (Array.isArray(value)) {
            value.forEach(item => {
              queryParams = queryParams.append(key, item.toString());
            });
          } else {
            queryParams = queryParams.append(key, value.toString());
          }
        }
      });
    }

    return this.http.get<TaskPaginatedResponseDto>(`${this.baseUrl}`, { params: queryParams });
  }

  public put(id: number, dto: TaskUpdateDto): Observable<TaskResponseDto> {
    return this.http.put<TaskResponseDto>(`${this.baseUrl}/${id}`, dto);
  }

  public patchStatus(id: number, dto: TaskStatusUpdateDto): Observable<TaskResponseDto> {
    return this.http.patch<TaskResponseDto>(`${this.baseUrl}/${id}/status`, dto);
  }
}
