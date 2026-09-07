/**
 * Task Management Module Models
 * Defines all interfaces for task operations, filtering, and responses
 */

// Enums for task statuses and priorities
export enum TaskStatus {
  Todo = 'To Do',
  InProgress = 'In Progress',
  OnHold = 'On Hold',
  Completed = 'Completed',
}

export enum TaskPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
}

// Attachment interface for reference documents
export interface TaskAttachment {
  id: string;
  fileName: string;
  fileType: string;
  fileSize: number;
  uploadDate: Date;
  downloadUrl?: string;
  uploadedByEmail?: string;
}

// Task model - main interface for task data
export interface Task {
  id: string;
  title: string;
  description: string;
  sprintName: string;
  category: string;
  priority: TaskPriority;
  status: TaskStatus;
  startDate: Date;
  dueDate: Date;
  assignedEmployeeId: string;
  assignedEmployeeName: string;
  assignedEmployeeEmail?: string;
  createdByEmployeeId: string;
  createdByEmployeeName: string;
  createdByEmail?: string;
  attachments: TaskAttachment[];
  createdAt: Date;
  updatedAt?: Date;
  completedAt?: Date;
  notes?: string;
}

// Response DTOs
export interface TaskDto {
  id: string;
  title: string;
  description: string;
  sprintName: string;
  category: string;
  priority: TaskPriority;
  status: TaskStatus;
  startDate: DateIso;
  dueDate: DateIso;
  assignedEmployeeId: string;
  assignedEmployeeName: string;
  assignedEmployeeEmail: string;
  createdByEmployeeId: string;
  createdByEmployeeName: string;
  createdByEmail: string;
  attachments: TaskAttachment[];
  createdAt: DateIso;
  updatedAt?: DateIso;
  completedAt?: DateIso;
  notes?: string;
}

export interface TaskListResponse {
  id: string;
  title: string;
  assignedEmployeeName: string;
  status: TaskStatus;
  priority: TaskPriority;
  dueDate: DateIso;
  category: string;
  sprintName: string;
}

export interface TaskDetailResponse extends TaskDto {}

// Request DTOs for API calls
export interface CreateTaskRequest {
  title: string;
  description: string;
  sprintName: string;
  category: string;
  priority: TaskPriority;
  status: TaskStatus;
  startDate: DateIso;
  dueDate: DateIso;
  assignedEmployeeId: string;
  notes?: string;
}

export interface UpdateTaskRequest {
  id: string;
  title?: string;
  description?: string;
  sprintName?: string;
  category?: string;
  priority?: TaskPriority;
  status?: TaskStatus;
  dueDate?: DateIso;
  assignedEmployeeId?: string;
  notes?: string;
}

export interface UpdateTaskStatusRequest {
  status: TaskStatus;
}

// Filtering and search
export interface TaskFilterCriteria {
  status?: TaskStatus[];
  priority?: TaskPriority[];
  assignedEmployeeId?: string[];
  category?: string[];
  sprintName?: string[];
  dateFrom?: DateIso;
  dateTo?: DateIso;
  search?: string;
  sortBy?: 'dueDate' | 'priority' | 'createdAt' | 'title';
  sortOrder?: 'asc' | 'desc';
  pageNumber?: number;
  pageSize?: number;
}

export interface PaginatedTaskResponse {
  items: TaskListResponse[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Dashboard summary
export interface TaskStatusSummary {
  [TaskStatus.Todo]: number;
  [TaskStatus.InProgress]: number;
  [TaskStatus.OnHold]: number;
  [TaskStatus.Completed]: number;
}

export interface TaskDashboardSummary {
  totalTasks: number;
  completedTasks: number;
  overdueTasks: number;
  statusSummary: TaskStatusSummary;
  recentTasks: TaskListResponse[];
}

// For attachment upload
export interface TaskAttachmentRequest {
  taskId: string;
  file: File;
}

// Helper type for date strings
type DateIso = string; // ISO 8601 format (e.g., "2024-01-15T10:30:00Z")

// Filter options for dropdowns
export interface FilterOptions {
  statuses: TaskStatus[];
  priorities: TaskPriority[];
  categories: string[];
  sprints: string[];
  employees: { id: string; name: string; email: string }[];
}
