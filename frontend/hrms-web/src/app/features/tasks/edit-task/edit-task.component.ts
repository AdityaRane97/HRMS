import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import { TasksService } from '../tasks.service';
import {
  UpdateTaskRequest,
  TaskStatus,
  TaskPriority,
  FilterOptions,
  TaskDetailResponse,
} from '../tasks.model';
import { TaskAttachmentsComponent } from '@shared/components/task-attachments.component';

@Component({
  selector: 'app-edit-task',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    TaskAttachmentsComponent,
  ],
  templateUrl: './edit-task.component.html',
  styleUrl: './edit-task.component.scss',
})
export class EditTaskComponent implements OnInit {
  UserRole = UserRole;

  currentUserRole: UserRole | null = null;
  currentUserId: string | null = null;
  taskId: string | null = null;

  // Form
  taskForm!: FormGroup;
  filterOptions: FilterOptions | null = null;
  currentTask: TaskDetailResponse | null = null;

  // State
  loading = true;
  isSubmitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  // Status and Priority dropdowns
  statusOptions = Object.values(TaskStatus);
  priorityOptions = Object.values(TaskPriority);

  // Attachments
  selectedFiles: File[] = [];

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private tasksService: TasksService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.route.params.subscribe((params) => {
      this.taskId = params['id'];
      if (this.taskId) {
        this.loadTaskAndInitializeForm();
      }
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

  private loadTaskAndInitializeForm(): void {
    this.tasksService.getTaskDetail(this.taskId!).subscribe({
      next: (task) => {
        this.currentTask = task;
        this.loadFilterOptions();
        this.initializeForm(task);
      },
      error: (err) => {
        console.error('Error loading task:', err);
        this.error = 'Failed to load task. Please try again.';
        this.loading = false;
      },
    });
  }

  private initializeForm(task: TaskDetailResponse): void {
    this.taskForm = this.formBuilder.group({
      title: [task.title, [Validators.required, Validators.minLength(5)]],
      description: [
        task.description,
        [Validators.required, Validators.minLength(10)],
      ],
      sprintName: [task.sprintName, Validators.required],
      category: [task.category, Validators.required],
      priority: [task.priority, Validators.required],
      status: [task.status, Validators.required],
      dueDate: [task.dueDate.split('T')[0], Validators.required],
      notes: [task.notes || ''],
    });

    this.loading = false;
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

  onAttachmentUpload(files: File[]): void {
    if (!this.taskId) return;

    files.forEach((file) => {
      // Validate file size (max 10MB)
      const maxSize = 10 * 1024 * 1024;
      if (file.size > maxSize) {
        this.error = `File "${file.name}" is too large. Maximum size is 10MB.`;
        return;
      }

      this.tasksService.uploadAttachment(this.taskId!, file).subscribe({
        next: (attachment) => {
          if (this.currentTask) {
            this.currentTask.attachments.push(attachment);
            this.successMessage = `"${file.name}" uploaded successfully`;
            setTimeout(() => {
              this.successMessage = null;
            }, 3000);
          }
        },
        error: (err) => {
          console.error('Error uploading attachment:', err);
          this.error = `Failed to upload "${file.name}"`;
        },
      });
    });
  }

  removeAttachment(attachmentId: string): void {
    if (!this.taskId) return;

    const confirmed = confirm('Remove this attachment?');
    if (!confirmed) return;

    this.tasksService.deleteAttachment(this.taskId, attachmentId).subscribe({
      next: () => {
        if (this.currentTask) {
          this.currentTask.attachments = this.currentTask.attachments.filter(
            (a) => a.id !== attachmentId
          );
          this.successMessage = 'Attachment removed successfully';
          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        }
      },
      error: (err) => {
        console.error('Error deleting attachment:', err);
        this.error = 'Failed to delete attachment';
      },
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.taskForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.taskForm.get(fieldName);
    if (!field || !field.errors) return '';

    if (field.errors['required']) return `${fieldName} is required`;
    if (field.errors['minlength'])
      return `${fieldName} must be at least ${field.errors['minlength'].requiredLength} characters`;
    if (field.errors['pattern']) return `${fieldName} format is invalid`;

    return 'Invalid field';
  }

  onSubmit(): void {
    if (!this.taskForm.valid || !this.taskId) {
      Object.keys(this.taskForm.controls).forEach((key) => {
        this.taskForm.get(key)?.markAsTouched();
      });
      this.error = 'Please fix the errors in the form';
      return;
    }

    this.isSubmitting = true;
    this.error = null;

    const formValue = this.taskForm.value;
    const request: UpdateTaskRequest = {
      id: this.taskId,
      title: formValue.title,
      description: formValue.description,
      sprintName: formValue.sprintName,
      category: formValue.category,
      priority: formValue.priority,
      status: formValue.status,
      dueDate: formValue.dueDate,
      notes: formValue.notes || undefined,
    };

    this.tasksService.updateTask(request).subscribe({
      next: (updatedTask) => {
        this.successMessage = 'Task updated successfully!';
        this.currentTask = updatedTask;
        this.isSubmitting = false;

        setTimeout(() => {
          this.router.navigate(['/tasks', this.taskId]);
        }, 1500);
      },
      error: (err) => {
        console.error('Error updating task:', err);
        this.error = 'Failed to update task. Please try again.';
        this.isSubmitting = false;
      },
    });
  }

  canEditTask(): boolean {
    return (
      this.currentUserRole === UserRole.Manager ||
      this.currentUserRole === UserRole.Admin
    );
  }

  cancel(): void {
    this.router.navigate(['/tasks', this.taskId]);
  }
}
