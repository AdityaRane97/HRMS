import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import { TasksService } from '../tasks.service';
import {
  Task,
  TaskListResponse,
  TaskFilterCriteria,
  TaskStatus,
  TaskPriority,
  FilterOptions,
  PaginatedTaskResponse,
} from '../tasks.model';
import { TaskStatusBadgeComponent } from '@shared/components/task-status-badge.component';
import { TaskPriorityBadgeComponent } from '@shared/components/task-priority-badge.component';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    TaskStatusBadgeComponent,
    TaskPriorityBadgeComponent,
  ],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.scss',
})
export class TaskListComponent implements OnInit {
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  currentUserId: string | null = null;

  // Data
  tasks: TaskListResponse[] = [];
  filterOptions: FilterOptions | null = null;

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 1;

  // Filters
  activeFilters: Partial<TaskFilterCriteria> = {
    pageNumber: 1,
    pageSize: 10,
  };

  searchQuery = '';
  selectedStatuses: TaskStatus[] = [];
  selectedPriorities: TaskPriority[] = [];
  selectedEmployees: string[] = [];
  selectedCategories: string[] = [];
  selectedSprints: string[] = [];
  dateFrom = '';
  dateTo = '';
  sortBy: 'dueDate' | 'priority' | 'createdAt' | 'title' = 'dueDate';
  sortOrder: 'asc' | 'desc' = 'asc';

  // UI state
  loading = true;
  error: string | null = null;
  showFilters = false;

  // Constants
  TaskStatus = TaskStatus;
  TaskPriority = TaskPriority;

  constructor(
    private authService: AuthService,
    private tasksService: TasksService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadFilterOptions();
    this.loadTasks();

    // Listen for query param changes (for view parameter)
    this.route.queryParams.subscribe(() => {
      this.loadTasks();
    });
  }

  private loadUserInfo(): void {
    this.authService.currentUser$.subscribe((user) => {
      if (user) {
        this.currentUserRole = user.role;
        this.currentUserId = user.id;
      }
    });
  }

  private loadFilterOptions(): void {
    this.tasksService.getFilterOptions().subscribe({
      next: (options) => {
        this.filterOptions = options;
      },
      error: (err) => {
        console.error('Error loading filter options:', err);
      },
    });
  }

  loadTasks(): void {
    this.loading = true;
    this.error = null;

    // Build filter criteria
    this.activeFilters = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      search: this.searchQuery.trim() || undefined,
      status: this.selectedStatuses.length > 0 ? this.selectedStatuses : undefined,
      priority:
        this.selectedPriorities.length > 0
          ? this.selectedPriorities
          : undefined,
      assignedEmployeeId:
        this.selectedEmployees.length > 0
          ? this.selectedEmployees
          : undefined,
      category:
        this.selectedCategories.length > 0 ? this.selectedCategories : undefined,
      sprintName:
        this.selectedSprints.length > 0 ? this.selectedSprints : undefined,
      dateFrom: this.dateFrom || undefined,
      dateTo: this.dateTo || undefined,
      sortBy: this.sortBy,
      sortOrder: this.sortOrder,
    };

    // Load tasks based on role
    const loadFn =
      this.currentUserRole === UserRole.Employee
        ? this.tasksService.getMyTasks.bind(
            this.tasksService,
            this.currentUserId || '',
            this.currentUserRole,
            this.activeFilters
          )
        : this.tasksService.getAllTasks.bind(
            this.tasksService,
            this.activeFilters
          );

    loadFn().subscribe({
      next: (response: PaginatedTaskResponse) => {
        this.tasks = response.items;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.currentPage = response.pageNumber;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading tasks:', err);
        this.error = 'Failed to load tasks. Please try again later.';
        this.loading = false;
      },
    });
  }

  onSearch(query: string): void {
    this.searchQuery = query;
    this.currentPage = 1;
    this.loadTasks();
  }

  toggleStatusFilter(status: TaskStatus): void {
    const index = this.selectedStatuses.indexOf(status);
    if (index > -1) {
      this.selectedStatuses.splice(index, 1);
    } else {
      this.selectedStatuses.push(status);
    }
    this.currentPage = 1;
    this.loadTasks();
  }

  togglePriorityFilter(priority: TaskPriority): void {
    const index = this.selectedPriorities.indexOf(priority);
    if (index > -1) {
      this.selectedPriorities.splice(index, 1);
    } else {
      this.selectedPriorities.push(priority);
    }
    this.currentPage = 1;
    this.loadTasks();
  }

  toggleEmployeeFilter(employeeId: string): void {
    const index = this.selectedEmployees.indexOf(employeeId);
    if (index > -1) {
      this.selectedEmployees.splice(index, 1);
    } else {
      this.selectedEmployees.push(employeeId);
    }
    this.currentPage = 1;
    this.loadTasks();
  }

  toggleCategoryFilter(category: string): void {
    const index = this.selectedCategories.indexOf(category);
    if (index > -1) {
      this.selectedCategories.splice(index, 1);
    } else {
      this.selectedCategories.push(category);
    }
    this.currentPage = 1;
    this.loadTasks();
  }

  toggleSprintFilter(sprint: string): void {
    const index = this.selectedSprints.indexOf(sprint);
    if (index > -1) {
      this.selectedSprints.splice(index, 1);
    } else {
      this.selectedSprints.push(sprint);
    }
    this.currentPage = 1;
    this.loadTasks();
  }

  applyDateFilter(): void {
    this.currentPage = 1;
    this.loadTasks();
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.selectedStatuses = [];
    this.selectedPriorities = [];
    this.selectedEmployees = [];
    this.selectedCategories = [];
    this.selectedSprints = [];
    this.dateFrom = '';
    this.dateTo = '';
    this.sortBy = 'dueDate';
    this.sortOrder = 'asc';
    this.currentPage = 1;
    this.loadTasks();
  }

  changeSortOrder(): void {
    this.sortOrder = this.sortOrder === 'asc' ? 'desc' : 'asc';
    this.loadTasks();
  }

  changePage(newPage: number): void {
    this.currentPage = Math.max(1, Math.min(newPage, this.totalPages));
    this.loadTasks();
  }

  getRowClass(task: TaskListResponse): string {
    switch (task.status) {
      case TaskStatus.Completed:
        return 'opacity-60';
      case TaskStatus.OnHold:
        return 'bg-yellow-50';
      default:
        return '';
    }
  }

  isOverdue(dueDate: string, status: TaskStatus): boolean {
    return (
      new Date(dueDate) < new Date() &&
      status !== TaskStatus.Completed
    );
  }

  hasActiveFilters(): boolean {
    return (
      this.searchQuery.length > 0 ||
      this.selectedStatuses.length > 0 ||
      this.selectedPriorities.length > 0 ||
      this.selectedEmployees.length > 0 ||
      this.selectedCategories.length > 0 ||
      this.selectedSprints.length > 0 ||
      this.dateFrom !== '' ||
      this.dateTo !== ''
    );
  }

  getFilterBadgeCount(): number {
    let count = 0;
    if (this.searchQuery.length > 0) count++;
    if (this.selectedStatuses.length > 0)
      count += this.selectedStatuses.length;
    if (this.selectedPriorities.length > 0)
      count += this.selectedPriorities.length;
    if (this.selectedEmployees.length > 0)
      count += this.selectedEmployees.length;
    if (this.selectedCategories.length > 0)
      count += this.selectedCategories.length;
    if (this.selectedSprints.length > 0) count += this.selectedSprints.length;
    if (this.dateFrom !== '') count++;
    if (this.dateTo !== '') count++;
    return count;
  }
}
