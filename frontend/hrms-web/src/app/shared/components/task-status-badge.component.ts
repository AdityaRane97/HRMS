import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskStatus } from '@features/tasks/tasks.model';

export interface StatusBadgeData {
  status: TaskStatus;
  count?: number;
}

@Component({
  selector: 'app-task-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [ngClass]="getStatusClass()" class="inline-flex items-center gap-2">
      <span class="h-2 w-2 rounded-full" [ngClass]="getStatusDotClass()"></span>
      {{ badge.status }}
      <span *ngIf="badge.count !== undefined" class="ml-1 text-xs font-bold">
        ({{ badge.count }})
      </span>
    </span>
  `,
  styles: [
    `
      :host ::ng-deep {
        .status-badge {
          padding: 0.375rem 0.75rem;
          border-radius: 0.375rem;
          font-size: 0.875rem;
          font-weight: 500;
          display: inline-flex;
          align-items: center;
          gap: 0.5rem;
        }
      }
    `,
  ],
})
export class TaskStatusBadgeComponent {
  @Input() badge!: StatusBadgeData;

  getStatusClass(): string {
    const baseClass =
      'px-3 py-1 rounded text-sm font-medium inline-flex items-center gap-1';
    switch (this.badge.status) {
      case TaskStatus.Completed:
        return `${baseClass} bg-green-100 text-green-800`;
      case TaskStatus.InProgress:
        return `${baseClass} bg-blue-100 text-blue-800`;
      case TaskStatus.OnHold:
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case TaskStatus.Todo:
        return `${baseClass} bg-gray-100 text-gray-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }

  getStatusDotClass(): string {
    switch (this.badge.status) {
      case TaskStatus.Completed:
        return 'bg-green-600';
      case TaskStatus.InProgress:
        return 'bg-blue-600';
      case TaskStatus.OnHold:
        return 'bg-yellow-600';
      case TaskStatus.Todo:
        return 'bg-gray-400';
      default:
        return 'bg-gray-400';
    }
  }
}
