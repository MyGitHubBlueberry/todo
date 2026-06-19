import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { CategoryCreateDto, CategoryResponseDto, CategoryUpdateDto } from "./types";
import { Observable } from "rxjs";

@Injectable({ providedIn: 'root' })
export class CategoryApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = import.meta.env.NG_APP_API_URL + '/categories';

  public create(dto: CategoryCreateDto): Observable<CategoryResponseDto> {
    return this.http.post<CategoryResponseDto>(this.baseUrl, dto);
  }

  public get(): Observable<CategoryResponseDto[]> {
    return this.http.get<CategoryResponseDto[]>(this.baseUrl);
  }

  public put(dto: CategoryUpdateDto): Observable<CategoryResponseDto> {
    return this.http.put<CategoryResponseDto>(this.baseUrl, dto);
  }

  public delete(ids: number[]): Observable<void> {
    return this.http.delete<void>(this.baseUrl, {
        body: { ids: ids }
    });
  }
}
