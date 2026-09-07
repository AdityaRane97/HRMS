import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import { TasksService } from './tasks.service';
import {
  TaskDashboardSummary,
  TaskStatus,
  TaskListResponse,
} from './tasks.model';
import { TaskSummaryCardComponent, TaskSummaryCardData } from '@shared/components/task-summary-card.component';
import { TaskStatusBadgeComponent } from '@shared/components/task-status-badge.component';
import { QuickAccessCardComponent, QuickAccessItemData } from '@shared/components/quick-access-card.component';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TaskSummaryCardComponent,
    TaskStatusBadgeComponent,
    QuickAccessCardComponent,
  ],
  templateUrl: './tasks.component.html',
  styleUrl: './tasks.component.scss',
})
export class TasksComponent implements OnInit {
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  currentUserId: string | null = null;

  // Dashboard data
  summary: TaskDashboardSummary | null = null;
  recentTasks: TaskListResponse[] = [];

  // UI state
  loading = true;
  error: string | null = null;

  // Summary cards
  summaryCards: TaskSummaryCardData[] = [];

  // Quick access items
  taskQuickAccess: QuickAccessItemData[] = [
    {
      title: 'All Tasks',
      description: 'View all tasks assigned to you',
      route: '/tasks/list',
      iconEmoji: '📋',
    },
    {
      title: 'My Tasks',
      description: 'Tasks assigned to me',
      route: '/tasks/my-tasks',
      iconEmoji: '👤',
    },
    {
      title: 'Create Task',
      description: 'Create a new task',
      route: '/tasks/create',
      iconEmoji: '✨',
    },
  ];

  // For manager/admin only
  managerTaskQuickAccess: QuickAccessItemData[] = [
    {
      title: 'All Tasks',
      description: 'View all tasks',
      route: '/tasks/list',
      iconEmoji: '📋',
    },
    {
      title: 'Create Task',
      description: 'Assign a new task',
      route: '/tasks/create',
      iconEmoji: '✨',
    },
    {
      title: 'Team Tasks',
      description: 'Tasks by team members',
      route: '/tasks/list?view=team',
      iconEmoji: '👥',
    },
  ];

  constructor(
    private authService: AuthService,
    private tasksService: TasksService
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadTaskDashboard();
  }

  private loadUserInfo(): void {
    this.authService.currentUser$.subscribe((user) => {
      if (user) {
        this.currentUserRole = user.role;
        this.currentUserId = user.id;
      }
    });
  }

  private loadTaskDashboard(): void {
    this.loading = true;
    this.error = null;

    this.tasksService
      .getTaskDashboardSummary(
        this.currentUserRole === UserRole.Employee
          ? this.currentUserId || undefined
          : undefined
      )
      .subscribe({
        next: (summary) => {
          this.summary = summary;
          this.buildSummaryCards();
          this.recentTasks = summary.recentTasks;
          this.loading = false;
        },
        error: (err) => {
          console.error('Error loading task dashboard:', err);
          this.error =
            'Failed to load task dashboard. Please try again later.';
          this.loading = false;
        },
      });
  }

  private buildSummaryCards(): void {
    if (!this.summary) return;

    this.summaryCards = [
      {
        title: 'Total Tasks',
        count: this.summary.totalTasks,
        icon: '📋',
        color: 'blue',
      },
      {
        title: 'In Progress',
        count: this.summary.statusSummary[TaskStatus.InProgress],
        icon: '⚙️',
        color: 'purple',
      },
      {
        title: 'Completed',
        count: this.summary.completedTasks,
        icon: '✅',
        color: 'green',
      },
      {
        title: 'Overdue',
        count: this.summary.overdueTasks,
        icon: '⚠️',
        color: 'red',
      },
    ];
  }

  getQuickAccessItems(): QuickAccessItemData[] {
    return this.currentUserRole === UserRole.Employee
      ? this.taskQuickAccess
      : this.managerTaskQuickAccess;
  }

  getStatusColor(status: TaskStatus): string {
    switch (status) {
      case TaskStatus.Completed:
        return 'green';
      case TaskStatus.InProgress:
        return 'blue';
      case TaskStatus.OnHold:
        return 'yellow';
      case TaskStatus.Todo:
        return 'gray';
      default:
        return 'gray';
    }
  }

  goToTaskList(): void {
    // Navigation handled by Router in template
  }
}
