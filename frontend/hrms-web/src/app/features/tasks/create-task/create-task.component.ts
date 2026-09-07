import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import { TasksService } from '../tasks.service';
import {
  CreateTaskRequest,
  TaskStatus,
  TaskPriority,
  FilterOptions,
} from '../tasks.model';
import { TaskAttachmentsComponent } from '@shared/components/task-attachments.component';

@Component({
  selector: 'app-create-task',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ReactiveFormsModule,
    TaskAttachmentsComponent,
  ],
  templateUrl: './create-task.component.html',
  styleUrl: './create-task.component.scss',
})
export class CreateTaskComponent implements OnInit {
  UserRole = UserRole;

  currentUserRole: UserRole | null = null;
  currentUserId: string | null = null;

  // Form
  taskForm!: FormGroup;
  filterOptions: FilterOptions | null = null;

  // State
  loading = false;
  isSubmitting = false;
  error: string | null = null;
  successMessage: string | null = null;

  // Status and Priority dropdowns
  statusOptions = Object.values(TaskStatus);
  priorityOptions = Object.values(TaskPriority);

  // Attachments
  selectedFiles: File[] = [];
  uploadedAttachments: { id: string; fileName: string }[] = [];

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private tasksService: TasksService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUserInfo();
    this.initializeForm();
    this.loadFilterOptions();
  }

  private loadUserInfo(): void {
    this.authService.currentUser$.subscribe((user) => {
      if (user) {
        this.currentUserRole = user.role;
        this.currentUserId = user.id;
      }
    });
  }

  private initializeForm(): void {
    const today = new Date().toISOString().split('T')[0];

    this.taskForm = this.formBuilder.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      description: ['', [Validators.required, Validators.minLength(10)]],
      sprintName: ['', Validators.required],
      category: ['', Validators.required],
      priority: [TaskPriority.Medium, Validators.required],
      status: [TaskStatus.Todo, Validators.required],
      startDate: [today, Validators.required],
      dueDate: [today, Validators.required],
      assignedEmployeeId: [
        this.currentUserRole === UserRole.Employee ? this.currentUserId : '',
        Validators.required,
      ],
      notes: [''],
    });
  }

  private loadFilterOptions(): void {
    this.loading = true;
    this.tasksService.getFilterOptions().subscribe({
      next: (options) => {
        this.filterOptions = options;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading filter options:', err);
        this.error = 'Failed to load form options. Please try again.';
        this.loading = false;
      },
    });
  }

  onAttachmentUpload(files: File[]): void {
    files.forEach((file) => {
      // Validate file size (max 10MB)
      const maxSize = 10 * 1024 * 1024;
      if (file.size > maxSize) {
        this.error = `File "${file.name}" is too large. Maximum size is 10MB.`;
        return;
      }

      this.selectedFiles.push(file);
      this.uploadedAttachments.push({
        id: Math.random().toString(36).substr(2, 9),
        fileName: file.name,
      });
    });
  }

  removeAttachment(attachmentId: string): void {
    const index = this.uploadedAttachments.findIndex(
      (a) => a.id === attachmentId
    );
    if (index > -1) {
      this.uploadedAttachments.splice(index, 1);
      this.selectedFiles.splice(index, 1);
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.taskForm.get(fieldName);
    return !!(
      field &&
      field.invalid &&
      (field.dirty || field.touched)
    );
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
    if (!this.taskForm.valid) {
      // Mark all fields as touched to show errors
      Object.keys(this.taskForm.controls).forEach((key) => {
        this.taskForm.get(key)?.markAsTouched();
      });
      this.error = 'Please fix the errors in the form';
      return;
    }

    this.isSubmitting = true;
    this.error = null;

    const formValue = this.taskForm.value;
    const request: CreateTaskRequest = {
      title: formValue.title,
      description: formValue.description,
      sprintName: formValue.sprintName,
      category: formValue.category,
      priority: formValue.priority,
      status: formValue.status,
      startDate: formValue.startDate,
      dueDate: formValue.dueDate,
      assignedEmployeeId: formValue.assignedEmployeeId,
      notes: formValue.notes || undefined,
    };

    this.tasksService.createTask(request).subscribe({
      next: (newTask) => {
        this.successMessage = 'Task created successfully!';

        // Upload attachments if any
        if (this.selectedFiles.length > 0) {
          this.uploadAttachments(newTask.id);
        } else {
          // Navigate back to task list after successful creation
          setTimeout(() => {
            this.router.navigate(['/tasks']);
          }, 1500);
        }

        this.isSubmitting = false;
      },
      error: (err) => {
        console.error('Error creating task:', err);
        this.error =
          'Failed to create task. Please check your input and try again.';
        this.isSubmitting = false;
      },
    });
  }

  private uploadAttachments(taskId: string): void {
    let uploadCount = 0;

    this.selectedFiles.forEach((file) => {
      this.tasksService.uploadAttachment(taskId, file).subscribe({
        next: () => {
          uploadCount++;
          if (uploadCount === this.selectedFiles.length) {
            // All attachments uploaded
            setTimeout(() => {
              this.router.navigate(['/tasks']);
            }, 1500);
          }
        },
        error: (err) => {
          console.error('Error uploading attachment:', err);
          this.error = `Failed to upload attachment: ${file.name}`;
        },
      });
    });
  }

  canAssignToOtherEmployees(): boolean {
    return (
      this.currentUserRole === UserRole.Manager ||
      this.currentUserRole === UserRole.Admin
    );
  }

  cancel(): void {
    this.router.navigate(['/tasks']);
  }
}
