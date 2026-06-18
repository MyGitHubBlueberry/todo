import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { CategoryCreateDto, CategoryResponseDto, CategoryUpdateDto } from "./types";
import { Observable } from "rxjs";

@Injectable({ providedIn: 'root' })
export class CategoryApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = 'http://localhost:5000/api/categories';

  public create(dto: CategoryCreateDto): Observable<CategoryResponseDto> {
    return this.http.post<CategoryResponseDto>(this.baseUrl, dto);
  }

  public get(): Observable<CategoryResponseDto[]> {
    return this.http.get<CategoryResponseDto[]>(this.baseUrl);
  }

  public put(dto: CategoryUpdateDto): Observable<CategoryResponseDto> {
    return this.http.put<CategoryResponseDto>(this.baseUrl, dto);
  }
  public delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
