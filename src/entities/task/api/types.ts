import { CategoryResponseDto } from "@entities/category/api/types";

export interface TaskCreateDto {
  title: string,
  body: string | null,
  categoryIds: number[],
}

export interface TaskUpdateDto {
  title: string,
  body: string | null,
  categoryIds: number[],
}

export interface TaskStatusUpdateDto {
  status: TaskStatus;
}

export interface TaskGetQueryDto {
    page: number,
    pageSize: number,
    sortBy: SortBy,
    categoryIds: number[] | null,
    selectedStatus: TaskStatus | null,
    searchTerm: string | null,
}

export interface TaskPaginatedResponseDto {
  tasks: TaskResponseDto[];
  totalCount: number;
  page: number;
}

export type TaskStatus = 'Done' | 'Pending';

export type SortBy =
  | 'AlphAsc'
  | 'AlphDsc'
  | 'CrtAsc'
  | 'CrtDsc'
  | 'UpdAsc'
  | 'UpdDsc';

export interface TaskResponseDto {
  id: number,
  title: string,
  body: string | null,
  status: TaskStatus,
  createdAt: Date,
  updatedAt: Date | null,
  isEdited: boolean,
  categories: CategoryResponseDto[],
}
