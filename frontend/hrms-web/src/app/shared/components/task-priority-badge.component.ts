import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskPriority } from '@features/tasks/tasks.model';

export interface PriorityBadgeData {
  priority: TaskPriority;
}

@Component({
  selector: 'app-task-priority-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span [ngClass]="getPriorityClass()">
      {{ getPriorityIcon() }} {{ badge.priority }}
    </span>
  `,
})
export class TaskPriorityBadgeComponent {
  @Input() badge!: PriorityBadgeData;

  getPriorityClass(): string {
    const baseClass =
      'px-3 py-1 rounded text-sm font-medium inline-flex items-center gap-2';
    switch (this.badge.priority) {
      case TaskPriority.High:
        return `${baseClass} bg-red-100 text-red-800`;
      case TaskPriority.Medium:
        return `${baseClass} bg-orange-100 text-orange-800`;
      case TaskPriority.Low:
        return `${baseClass} bg-green-100 text-green-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }

  getPriorityIcon(): string {
    switch (this.badge.priority) {
      case TaskPriority.High:
        return '🔴';
      case TaskPriority.Medium:
        return '🟡';
      case TaskPriority.Low:
        return '🟢';
      default:
        return '⚪';
    }
  }
}
