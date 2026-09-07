import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { API_CONFIG } from '@core/config/api.config';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import {
  Task,
  TaskDto,
  TaskListResponse,
  TaskDetailResponse,
  CreateTaskRequest,
  UpdateTaskRequest,
  UpdateTaskStatusRequest,
  TaskFilterCriteria,
  PaginatedTaskResponse,
  TaskDashboardSummary,
  TaskStatus,
  TaskPriority,
  TaskStatusSummary,
  FilterOptions,
  TaskAttachment,
} from './tasks.model';

@Injectable({
  providedIn: 'root',
})
export class TasksService {
  private tasksSubject = new BehaviorSubject<Task[]>([]);
  public tasks$ = this.tasksSubject.asObservable();

  private mockTasks: Task[] = [
    {
      id: '1',
      title: 'Complete Q4 Performance Review',
      description: 'Finalize performance review documentation and feedback for all team members.',
      sprintName: 'Sprint 23',
      category: 'HR',
      priority: TaskPriority.High,
      status: TaskStatus.InProgress,
      startDate: new Date('2024-01-01'),
      dueDate: new Date('2024-01-15'),
      assignedEmployeeId: 'emp-001',
      assignedEmployeeName: 'John Doe',
      assignedEmployeeEmail: 'john.doe@company.com',
      createdByEmployeeId: 'emp-manager-001',
      createdByEmployeeName: 'Jane Smith',
      createdByEmail: 'jane.smith@company.com',
      attachments: [
        {
          id: 'att-1',
          fileName: 'Performance_Criteria.pdf',
          fileType: 'application/pdf',
          fileSize: 2048576,
          uploadDate: new Date('2024-01-02'),
          uploadedByEmail: 'jane.smith@company.com',
        },
      ],
      createdAt: new Date('2024-01-01'),
      updatedAt: new Date('2024-01-05'),
      notes: 'Follow company guidelines and ensure timely completion',
    },
    {
      id: '2',
      title: 'Update API Documentation',
      description: 'Update REST API endpoint documentation with new v2 endpoints.',
      sprintName: 'Sprint 23',
      category: 'Development',
      priority: TaskPriority.Medium,
      status: TaskStatus.Todo,
      startDate: new Date('2024-01-10'),
      dueDate: new Date('2024-01-20'),
      assignedEmployeeId: 'emp-002',
      assignedEmployeeName: 'Robert Wilson',
      assignedEmployeeEmail: 'robert.wilson@company.com',
      createdByEmployeeId: 'emp-manager-001',
      createdByEmployeeName: 'Jane Smith',
      createdByEmail: 'jane.smith@company.com',
      attachments: [],
      createdAt: new Date('2024-01-08'),
    },
    {
      id: '3',
      title: 'Prepare Budget Report',
      description: 'Compile quarterly budget analysis and departmental expenses.',
      sprintName: 'Sprint 24',
      category: 'Finance',
      priority: TaskPriority.High,
      status: TaskStatus.OnHold,
      startDate: new Date('2024-01-05'),
      dueDate: new Date('2024-01-12'),
      assignedEmployeeId: 'emp-003',
      assignedEmployeeName: 'Sarah Johnson',
      assignedEmployeeEmail: 'sarah.johnson@company.com',
      createdByEmployeeId: 'emp-manager-002',
      createdByEmployeeName: 'Mike Chen',
      createdByEmail: 'mike.chen@company.com',
      attachments: [
        {
          id: 'att-2',
          fileName: 'Budget_Template_Q4.xlsx',
          fileType: 'application/vnd.ms-excel',
          fileSize: 512000,
          uploadDate: new Date('2024-01-05'),
          uploadedByEmail: 'mike.chen@company.com',
        },
      ],
      createdAt: new Date('2024-01-04'),
      updatedAt: new Date('2024-01-06'),
      notes: 'Awaiting data from accounting department',
    },
    {
      id: '4',
      title: 'Client Presentation Preparation',
      description: 'Create presentation slides for quarterly client meeting.',
      sprintName: 'Sprint 23',
      category: 'Sales',
      priority: TaskPriority.High,
      status: TaskStatus.InProgress,
      startDate: new Date('2024-01-08'),
      dueDate: new Date('2024-01-18'),
      assignedEmployeeId: 'emp-004',
      assignedEmployeeName: 'Emily Brown',
      assignedEmployeeEmail: 'emily.brown@company.com',
      createdByEmployeeId: 'emp-manager-001',
      createdByEmployeeName: 'Jane Smith',
      createdByEmail: 'jane.smith@company.com',
      attachments: [],
      createdAt: new Date('2024-01-07'),
      updatedAt: new Date('2024-01-10'),
    },
    {
      id: '5',
      title: 'Code Review - Authentication Module',
      description: 'Perform code review on new authentication module implementation.',
      sprintName: 'Sprint 23',
      category: 'Development',
      priority: TaskPriority.Medium,
      status: TaskStatus.Completed,
      startDate: new Date('2023-12-28'),
      dueDate: new Date('2024-01-05'),
      assignedEmployeeId: 'emp-002',
      assignedEmployeeName: 'Robert Wilson',
      assignedEmployeeEmail: 'robert.wilson@company.com',
      createdByEmployeeId: 'emp-manager-001',
      createdByEmployeeName: 'Jane Smith',
      createdByEmail: 'jane.smith@company.com',
      attachments: [],
      createdAt: new Date('2023-12-27'),
      updatedAt: new Date('2024-01-05'),
      completedAt: new Date('2024-01-05'),
      notes: 'Successfully completed with minor feedback',
    },
    {
      id: '6',
      title: 'Team Training - New Tools',
      description: 'Conduct training session on new project management tools.',
      sprintName: 'Sprint 24',
      category: 'Training',
      priority: TaskPriority.Low,
      status: TaskStatus.Todo,
      startDate: new Date('2024-01-22'),
      dueDate: new Date('2024-02-01'),
      assignedEmployeeId: 'emp-005',
      assignedEmployeeName: 'David Miller',
      assignedEmployeeEmail: 'david.miller@company.com',
      createdByEmployeeId: 'emp-manager-002',
      createdByEmployeeName: 'Mike Chen',
      createdByEmail: 'mike.chen@company.com',
      attachments: [
        {
          id: 'att-3',
          fileName: 'Training_Materials.zip',
          fileType: 'application/zip',
          fileSize: 5242880,
          uploadDate: new Date('2024-01-20'),
          uploadedByEmail: 'mike.chen@company.com',
        },
      ],
      createdAt: new Date('2024-01-18'),
    },
  ];

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    // Initialize with mock data
    this.tasksSubject.next(this.mockTasks);
  }

  /**
   * Get all tasks (for managers/admins) or tasks assigned to current employee
   */
  getMyTasks(
    currentEmployeeId: string,
    userRole: UserRole | null,
    filterCriteria?: Partial<TaskFilterCriteria>
  ): Observable<PaginatedTaskResponse> {
    // TODO: Replace with API call
    // const params = this.buildFilterParams(filterCriteria);
    // return this.http.get<ApiResponse<PaginatedTaskResponse>>(
    //   `${API_CONFIG.baseUrl}/tasks/my-tasks`,
    //   { params }
    // ).pipe(map(r => r.data));

    // For now, return mock data filtered by role
    let tasks = [...this.mockTasks];

    // Employees only see their assigned tasks
    if (userRole === UserRole.Employee) {
      tasks = tasks.filter((t) => t.assignedEmployeeId === currentEmployeeId);
    }

    // Apply filters if provided
    if (filterCriteria) {
      tasks = this.applyFilters(tasks, filterCriteria);
    }

    const response: PaginatedTaskResponse = {
      items: tasks.map(this.taskToListResponse),
      totalCount: tasks.length,
      pageNumber: filterCriteria?.pageNumber || 1,
      pageSize: filterCriteria?.pageSize || 10,
      totalPages: Math.ceil(
        tasks.length / (filterCriteria?.pageSize || 10)
      ),
    };

    return of(response);
  }

  /**
   * Get all tasks (admin/manager view with pagination)
   */
  getAllTasks(
    filterCriteria?: Partial<TaskFilterCriteria>
  ): Observable<PaginatedTaskResponse> {
    // TODO: Replace with API call
    // const params = this.buildFilterParams(filterCriteria);
    // return this.http.get<ApiResponse<PaginatedTaskResponse>>(
    //   `${API_CONFIG.baseUrl}/tasks`,
    //   { params }
    // ).pipe(map(r => r.data));

    let tasks = [...this.mockTasks];

    if (filterCriteria) {
      tasks = this.applyFilters(tasks, filterCriteria);
    }

    const pageSize = filterCriteria?.pageSize || 10;
    const pageNumber = filterCriteria?.pageNumber || 1;
    const start = (pageNumber - 1) * pageSize;
    const paginatedTasks = tasks.slice(start, start + pageSize);

    const response: PaginatedTaskResponse = {
      items: paginatedTasks.map(this.taskToListResponse),
      totalCount: tasks.length,
      pageNumber,
      pageSize,
      totalPages: Math.ceil(tasks.length / pageSize),
    };

    return of(response);
  }

  /**
   * Get task detail by ID
   */
  getTaskDetail(taskId: string): Observable<TaskDetailResponse> {
    // TODO: Replace with API call
    // return this.http.get<ApiResponse<TaskDetailResponse>>(
    //   `${API_CONFIG.baseUrl}/tasks/${taskId}`
    // ).pipe(map(r => r.data));

    const task = this.mockTasks.find((t) => t.id === taskId);
    if (!task) {
      throw new Error(`Task with ID ${taskId} not found`);
    }

    return of(this.taskToDetailResponse(task));
  }

  /**
   * Create a new task (manager/admin only)
   */
  createTask(request: CreateTaskRequest): Observable<TaskDetailResponse> {
    // TODO: Replace with API call
    // return this.http.post<ApiResponse<TaskDetailResponse>>(
    //   `${API_CONFIG.baseUrl}/tasks`,
    //   request
    // ).pipe(
    //   map(r => r.data),
    //   tap(newTask => {
    //     const tasks = this.tasksSubject.value;
    //     this.tasksSubject.next([...tasks, newTask as Task]);
    //   })
    // );

    const newTask: Task = {
      id: Math.random().toString(36).substr(2, 9),
      title: request.title,
      description: request.description,
      sprintName: request.sprintName,
      category: request.category,
      priority: request.priority,
      status: request.status,
      startDate: new Date(request.startDate),
      dueDate: new Date(request.dueDate),
      assignedEmployeeId: request.assignedEmployeeId,
      assignedEmployeeName: 'Assigned Employee', // This would come from service call
      assignedEmployeeEmail: 'email@company.com',
      createdByEmployeeId: '', // Should come from auth service
      createdByEmployeeName: '',
      createdByEmail: '',
      attachments: [],
      createdAt: new Date(),
      notes: request.notes,
    };

    const tasks = this.tasksSubject.value;
    this.tasksSubject.next([...tasks, newTask]);

    return of(this.taskToDetailResponse(newTask));
  }

  /**
   * Update task details (manager/admin for most fields, employees can update status only)
   */
  updateTask(request: UpdateTaskRequest): Observable<TaskDetailResponse> {
    // TODO: Replace with API call
    // return this.http.put<ApiResponse<TaskDetailResponse>>(
    //   `${API_CONFIG.baseUrl}/tasks/${request.id}`,
    //   request
    // ).pipe(
    //   map(r => r.data),
    //   tap(updatedTask => {
    //     const tasks = this.tasksSubject.value.map(t =>
    //       t.id === updatedTask.id ? (updatedTask as Task) : t
    //     );
    //     this.tasksSubject.next(tasks);
    //   })
    // );

    const taskIndex = this.mockTasks.findIndex((t) => t.id === request.id);
    if (taskIndex === -1) {
      throw new Error(`Task with ID ${request.id} not found`);
    }

    const baseTask = this.mockTasks[taskIndex];
    const updatedTask: Task = {
      ...baseTask,
      title: request.title ?? baseTask.title,
      description: request.description ?? baseTask.description,
      sprintName: request.sprintName ?? baseTask.sprintName,
      category: request.category ?? baseTask.category,
      priority: request.priority ?? baseTask.priority,
      status: request.status ?? baseTask.status,
      dueDate: request.dueDate ? new Date(request.dueDate) : baseTask.dueDate,
      notes: request.notes ?? baseTask.notes,
      updatedAt: new Date(),
    };

    this.mockTasks[taskIndex] = updatedTask;
    const tasks = [...this.mockTasks];
    this.tasksSubject.next(tasks);

    return of(this.taskToDetailResponse(updatedTask));
  }

  /**
   * Update task status (employees can only update status)
   */
  updateTaskStatus(
    taskId: string,
    statusUpdate: UpdateTaskStatusRequest
  ): Observable<TaskDetailResponse> {
    // TODO: Replace with API call
    // return this.http.patch<ApiResponse<TaskDetailResponse>>(
    //   `${API_CONFIG.baseUrl}/tasks/${taskId}/status`,
    //   statusUpdate
    // ).pipe(map(r => r.data));

    return this.updateTask({
      id: taskId,
      status: statusUpdate.status,
    });
  }

  /**
   * Delete/cancel task (manager/admin only)
   */
  deleteTask(taskId: string): Observable<void> {
    // TODO: Replace with API call
    // return this.http.delete<void>(
    //   `${API_CONFIG.baseUrl}/tasks/${taskId}`
    // ).pipe(
    //   tap(() => {
    //     const tasks = this.tasksSubject.value.filter(t => t.id !== taskId);
    //     this.tasksSubject.next(tasks);
    //   })
    // );

    const tasks = this.mockTasks.filter((t) => t.id !== taskId);
    this.mockTasks = tasks;
    this.tasksSubject.next(tasks);

    return of(void 0);
  }

  /**
   * Get dashboard summary with task counts by status
   */
  getTaskDashboardSummary(
    employeeId?: string
  ): Observable<TaskDashboardSummary> {
    // TODO: Replace with API call
    // return this.http.get<ApiResponse<TaskDashboardSummary>>(
    //   `${API_CONFIG.baseUrl}/tasks/dashboard/summary`,
    //   { params: employeeId ? { employeeId } : undefined }
    // ).pipe(map(r => r.data));

    let tasks = [...this.mockTasks];
    if (employeeId) {
      tasks = tasks.filter((t) => t.assignedEmployeeId === employeeId);
    }

    const statusSummary: TaskStatusSummary = {
      [TaskStatus.Todo]: tasks.filter(
        (t) => t.status === TaskStatus.Todo
      ).length,
      [TaskStatus.InProgress]: tasks.filter(
        (t) => t.status === TaskStatus.InProgress
      ).length,
      [TaskStatus.OnHold]: tasks.filter(
        (t) => t.status === TaskStatus.OnHold
      ).length,
      [TaskStatus.Completed]: tasks.filter(
        (t) => t.status === TaskStatus.Completed
      ).length,
    };

    const overdueTasks = tasks.filter(
      (t) => new Date(t.dueDate) < new Date() && t.status !== TaskStatus.Completed
    ).length;

    const summary: TaskDashboardSummary = {
      totalTasks: tasks.length,
      completedTasks: statusSummary[TaskStatus.Completed],
      overdueTasks,
      statusSummary,
      recentTasks: tasks
        .sort(
          (a, b) =>
            new Date(b.createdAt).getTime() -
            new Date(a.createdAt).getTime()
        )
        .slice(0, 5)
        .map(this.taskToListResponse),
    };

    return of(summary);
  }

  /**
   * Get filter options for dropdowns
   */
  getFilterOptions(): Observable<FilterOptions> {
    // TODO: Replace with API call to get employees, sprints, etc.
    // return this.http.get<ApiResponse<FilterOptions>>(
    //   `${API_CONFIG.baseUrl}/tasks/filter-options`
    // ).pipe(map(r => r.data));

    const employees = [
      { id: 'emp-001', name: 'John Doe', email: 'john.doe@company.com' },
      { id: 'emp-002', name: 'Robert Wilson', email: 'robert.wilson@company.com' },
      { id: 'emp-003', name: 'Sarah Johnson', email: 'sarah.johnson@company.com' },
      { id: 'emp-004', name: 'Emily Brown', email: 'emily.brown@company.com' },
      { id: 'emp-005', name: 'David Miller', email: 'david.miller@company.com' },
    ];

    return of({
      statuses: Object.values(TaskStatus),
      priorities: Object.values(TaskPriority),
      categories: [
        'Development',
        'HR',
        'Finance',
        'Sales',
        'Training',
        'Other',
      ],
      sprints: ['Sprint 22', 'Sprint 23', 'Sprint 24', 'Sprint 25'],
      employees,
    });
  }

  /**
   * Download attachment
   */
  downloadAttachment(
    taskId: string,
    attachmentId: string
  ): Observable<Blob> {
    // TODO: Replace with API call
    // return this.http.get(
    //   `${API_CONFIG.baseUrl}/tasks/${taskId}/attachments/${attachmentId}/download`,
    //   { responseType: 'blob' }
    // );

    return of(new Blob());
  }

  /**
   * Upload attachment to task
   */
  uploadAttachment(
    taskId: string,
    file: File
  ): Observable<TaskAttachment> {
    // TODO: Replace with FormData HTTP call
    // const formData = new FormData();
    // formData.append('file', file);
    // return this.http.post<ApiResponse<TaskAttachment>>(
    //   `${API_CONFIG.baseUrl}/tasks/${taskId}/attachments`,
    //   formData
    // ).pipe(map(r => r.data));

    const attachment: TaskAttachment = {
      id: Math.random().toString(36).substr(2, 9),
      fileName: file.name,
      fileType: file.type,
      fileSize: file.size,
      uploadDate: new Date(),
    };

    return of(attachment);
  }

  /**
   * Remove attachment from task
   */
  deleteAttachment(
    taskId: string,
    attachmentId: string
  ): Observable<void> {
    // TODO: Replace with API call
    // return this.http.delete<void>(
    //   `${API_CONFIG.baseUrl}/tasks/${taskId}/attachments/${attachmentId}`
    // );

    return of(void 0);
  }

  // ======== Private Helper Methods ========

  private applyFilters(
    tasks: Task[],
    criteria: Partial<TaskFilterCriteria>
  ): Task[] {
    let filtered = [...tasks];

    if (criteria.status && criteria.status.length > 0) {
      filtered = filtered.filter((t) => criteria.status!.includes(t.status));
    }

    if (criteria.priority && criteria.priority.length > 0) {
      filtered = filtered.filter((t) =>
        criteria.priority!.includes(t.priority)
      );
    }

    if (criteria.assignedEmployeeId && criteria.assignedEmployeeId.length > 0) {
      filtered = filtered.filter((t) =>
        criteria.assignedEmployeeId!.includes(t.assignedEmployeeId)
      );
    }

    if (criteria.category && criteria.category.length > 0) {
      filtered = filtered.filter((t) => criteria.category!.includes(t.category));
    }

    if (criteria.sprintName && criteria.sprintName.length > 0) {
      filtered = filtered.filter((t) =>
        criteria.sprintName!.includes(t.sprintName)
      );
    }

    if (criteria.dateFrom) {
      const fromDate = new Date(criteria.dateFrom);
      filtered = filtered.filter((t) => new Date(t.dueDate) >= fromDate);
    }

    if (criteria.dateTo) {
      const toDate = new Date(criteria.dateTo);
      filtered = filtered.filter((t) => new Date(t.dueDate) <= toDate);
    }

    if (criteria.search && criteria.search.trim()) {
      const search = criteria.search.toLowerCase();
      filtered = filtered.filter(
        (t) =>
          t.title.toLowerCase().includes(search) ||
          t.description.toLowerCase().includes(search) ||
          t.category.toLowerCase().includes(search)
      );
    }

    // Sorting
    if (criteria.sortBy) {
      const sortOrder = criteria.sortOrder === 'desc' ? -1 : 1;
      filtered.sort((a, b) => {
        let aValue: any = a[criteria.sortBy as keyof Task];
        let bValue: any = b[criteria.sortBy as keyof Task];

        if (aValue instanceof Date && bValue instanceof Date) {
          return (aValue.getTime() - bValue.getTime()) * sortOrder;
        }

        if (typeof aValue === 'string' && typeof bValue === 'string') {
          return aValue.localeCompare(bValue) * sortOrder;
        }

        return (aValue - bValue) * sortOrder;
      });
    }

    return filtered;
  }

  private taskToListResponse(task: Task): TaskListResponse {
    return {
      id: task.id,
      title: task.title,
      assignedEmployeeName: task.assignedEmployeeName,
      status: task.status,
      priority: task.priority,
      dueDate: task.dueDate.toISOString(),
      category: task.category,
      sprintName: task.sprintName,
    };
  }

  private taskToDetailResponse(task: Task): TaskDetailResponse {
    return {
      id: task.id,
      title: task.title,
      description: task.description,
      sprintName: task.sprintName,
      category: task.category,
      priority: task.priority,
      status: task.status,
      startDate: task.startDate.toISOString(),
      dueDate: task.dueDate.toISOString(),
      assignedEmployeeId: task.assignedEmployeeId,
      assignedEmployeeName: task.assignedEmployeeName,
      assignedEmployeeEmail: task.assignedEmployeeEmail || '',
      createdByEmployeeId: task.createdByEmployeeId,
      createdByEmployeeName: task.createdByEmployeeName,
      createdByEmail: task.createdByEmail || '',
      attachments: task.attachments,
      createdAt: task.createdAt.toISOString(),
      updatedAt: task.updatedAt?.toISOString(),
      completedAt: task.completedAt?.toISOString(),
      notes: task.notes,
    };
  }

  private buildFilterParams(
    criteria?: Partial<TaskFilterCriteria>
  ): HttpParams {
    let params = new HttpParams();

    if (!criteria) return params;

    if (criteria.status && criteria.status.length > 0) {
      params = params.set('statuses', criteria.status.join(','));
    }
    if (criteria.priority && criteria.priority.length > 0) {
      params = params.set('priorities', criteria.priority.join(','));
    }
    if (criteria.assignedEmployeeId && criteria.assignedEmployeeId.length > 0) {
      params = params.set('employeeIds', criteria.assignedEmployeeId.join(','));
    }
    if (criteria.category && criteria.category.length > 0) {
      params = params.set('categories', criteria.category.join(','));
    }
    if (criteria.sprintName && criteria.sprintName.length > 0) {
      params = params.set('sprints', criteria.sprintName.join(','));
    }
    if (criteria.dateFrom) {
      params = params.set('dateFrom', criteria.dateFrom);
    }
    if (criteria.dateTo) {
      params = params.set('dateTo', criteria.dateTo);
    }
    if (criteria.search) {
      params = params.set('search', criteria.search);
    }
    if (criteria.sortBy) {
      params = params.set('sortBy', criteria.sortBy);
    }
    if (criteria.sortOrder) {
      params = params.set('sortOrder', criteria.sortOrder);
    }
    if (criteria.pageNumber) {
      params = params.set('pageNumber', criteria.pageNumber.toString());
    }
    if (criteria.pageSize) {
      params = params.set('pageSize', criteria.pageSize.toString());
    }

    return params;
  }
}
