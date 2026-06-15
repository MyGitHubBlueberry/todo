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

export enum TaskStatus {
  Done,
  Pending
}

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
