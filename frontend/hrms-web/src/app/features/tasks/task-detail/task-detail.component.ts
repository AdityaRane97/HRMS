import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import { TasksService } from '../tasks.service';
import {
  TaskDetailResponse,
  TaskStatus,
  UpdateTaskStatusRequest,
  TaskAttachment,
} from '../tasks.model';
import { TaskStatusBadgeComponent } from '@shared/components/task-status-badge.component';
import { TaskPriorityBadgeComponent } from '@shared/components/task-priority-badge.component';
import { TaskAttachmentsComponent } from '@shared/components/task-attachments.component';

@Component({
  selector: 'app-task-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TaskStatusBadgeComponent,
    TaskPriorityBadgeComponent,
    TaskAttachmentsComponent,
  ],
  templateUrl: './task-detail.component.html',
  styleUrl: './task-detail.component.scss',
})
export class TaskDetailComponent implements OnInit {
  UserRole = UserRole;
  TaskStatus = TaskStatus;

  currentUserRole: UserRole | null = null;
  currentUserId: string | null = null;
  taskId: string | null = null;

  // Data
  task: TaskDetailResponse | null = null;

  // UI state
  loading = true;
  error: string | null = null;
  isUpdatingStatus = false;
  updateMessage: string | null = null;

  // Status options
  statusOptions = [
    TaskStatus.Todo,
    TaskStatus.InProgress,
    TaskStatus.OnHold,
    TaskStatus.Completed,
  ];

  constructor(
    private authService: AuthService,
    private tasksService: TasksService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.loadTaskDetail();
  }

  private loadUserInfo(): void {
    this.authService.currentUser$.subscribe((user) => {
      if (user) {
        this.currentUserRole = user.role;
        this.currentUserId = user.id;
      }
    });
  }

  private loadTaskDetail(): void {
    this.route.params.subscribe((params) => {
      this.taskId = params['id'];
      if (this.taskId) {
        this.loading = true;
        this.error = null;

        this.tasksService.getTaskDetail(this.taskId).subscribe({
          next: (task) => {
            this.task = task;
            this.loading = false;
          },
          error: (err) => {
            console.error('Error loading task:', err);
            this.error =
              'Failed to load task details. Please try again later.';
            this.loading = false;
          },
        });
      }
    });
  }

  isTaskAssignedToCurrentUser(): boolean {
    return (
      this.currentUserRole === UserRole.Employee &&
      this.task?.assignedEmployeeId === this.currentUserId
    );
  }

  canEditTask(): boolean {
    return this.currentUserRole !== UserRole.Employee;
  }

  canUpdateStatus(): boolean {
    // Employees can update status of assigned tasks
    if (this.currentUserRole === UserRole.Employee) {
      return this.isTaskAssignedToCurrentUser();
    }
    // Managers and admins can update any task
    return this.currentUserRole === UserRole.Manager || this.currentUserRole === UserRole.Admin;
  }

  updateTaskStatus(newStatus: TaskStatus): void {
    if (!this.taskId || !this.canUpdateStatus()) return;

    this.isUpdatingStatus = true;
    this.error = null;
    this.updateMessage = null;

    const request: UpdateTaskStatusRequest = { status: newStatus };

    this.tasksService.updateTaskStatus(this.taskId, request).subscribe({
      next: (updatedTask) => {
        this.task = updatedTask;
        this.updateMessage = `Task status updated to ${newStatus}`;
        this.isUpdatingStatus = false;

        // Clear message after 3 seconds
        setTimeout(() => {
          this.updateMessage = null;
        }, 3000);
      },
      error: (err) => {
        console.error('Error updating task status:', err);
        this.error = 'Failed to update task status. Please try again.';
        this.isUpdatingStatus = false;
      },
    });
  }

  deleteTask(): void {
    if (!this.taskId || !this.canEditTask()) return;

    const confirmed = confirm('Are you sure you want to delete this task?');
    if (!confirmed) return;

    this.tasksService.deleteTask(this.taskId).subscribe({
      next: () => {
        this.router.navigate(['/tasks']);
      },
      error: (err) => {
        console.error('Error deleting task:', err);
        this.error = 'Failed to delete task. Please try again.';
      },
    });
  }

  editTask(): void {
    if (this.taskId) {
      this.router.navigate(['/tasks', this.taskId, 'edit']);
    }
  }

  downloadAttachment(attachment: TaskAttachment): void {
    if (!this.taskId) return;

    this.tasksService
      .downloadAttachment(this.taskId, attachment.id)
      .subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.download = attachment.fileName;
          link.click();
          window.URL.revokeObjectURL(url);
        },
        error: (err) => {
          console.error('Error downloading attachment:', err);
          this.error = 'Failed to download attachment.';
        },
      });
  }

  deleteAttachment(attachment: TaskAttachment): void {
    if (!this.taskId || !this.canEditTask()) return;

    const confirmed = confirm(
      `Are you sure you want to remove "${attachment.fileName}"?`
    );
    if (!confirmed) return;

    this.tasksService.deleteAttachment(this.taskId, attachment.id).subscribe({
      next: () => {
        if (this.task) {
          this.task.attachments = this.task.attachments.filter(
            (a) => a.id !== attachment.id
          );
        }
        this.updateMessage = 'Attachment removed successfully';
        setTimeout(() => {
          this.updateMessage = null;
        }, 3000);
      },
      error: (err) => {
        console.error('Error deleting attachment:', err);
        this.error = 'Failed to delete attachment.';
      },
    });
  }

  onAttachmentUpload(files: File[]): void {
    if (!this.taskId || !this.canEditTask()) return;

    files.forEach((file) => {
      this.tasksService.uploadAttachment(this.taskId!, file).subscribe({
        next: (attachment) => {
          if (this.task) {
            this.task.attachments.push(attachment);
          }
          this.updateMessage = `"${file.name}" uploaded successfully`;
          setTimeout(() => {
            this.updateMessage = null;
          }, 3000);
        },
        error: (err) => {
          console.error('Error uploading attachment:', err);
          this.error = `Failed to upload "${file.name}"`;
        },
      });
    });
  }

  isOverdue(): boolean {
    if (!this.task) return false;
    return (
      new Date(this.task.dueDate) < new Date() &&
      this.task.status !== TaskStatus.Completed
    );
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  }

  getDaysUntilDue(): number {
    if (!this.task) return 0;
    const dueDate = new Date(this.task.dueDate);
    const today = new Date();
    const diffTime = dueDate.getTime() - today.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }

  getDaysUntilDueStatus(): string {
    const days = this.getDaysUntilDue();
    if (days < 0) return `${Math.abs(days)} day(s) overdue`;
    if (days === 0) return 'Due today';
    if (days === 1) return '1 day left';
    return `${days} days left`;
  }
}
