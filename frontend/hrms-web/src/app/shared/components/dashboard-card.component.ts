import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface DashboardCardData {
  title: string;
  value: string | number;
  subtitle?: string;
  icon?: string;
  color?: 'primary' | 'success' | 'warning' | 'danger' | 'info';
  badge?: string;
}

@Component({
  selector: 'app-dashboard-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      [ngClass]="'bg-white rounded-lg shadow p-6 hover:shadow-md transition-shadow border-l-4 ' + getBorderColor()"
    >
      <div class="flex justify-between items-start">
        <div class="flex-1">
          <p class="text-sm text-gray-600 mb-2">{{ data.title }}</p>
          <p class="text-3xl font-bold text-gray-900">{{ data.value }}</p>
          <p *ngIf="data.subtitle" class="text-xs text-gray-500 mt-2">{{ data.subtitle }}</p>
        </div>
        <span *ngIf="data.badge" [ngClass]="'inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ' + getBadgeClass()">
          {{ data.badge }}
        </span>
      </div>
    </div>
  `,
  styles: [],
})
export class DashboardCardComponent {
  @Input() data!: DashboardCardData;

  getBorderColor(): string {
    switch (this.data.color) {
      case 'success':
        return 'border-green-500';
      case 'warning':
        return 'border-yellow-500';
      case 'danger':
        return 'border-red-500';
      case 'info':
        return 'border-blue-500';
      default:
        return 'border-primary-500';
    }
  }

  getBadgeClass(): string {
    switch (this.data.color) {
      case 'success':
        return 'bg-green-100 text-green-800';
      case 'warning':
        return 'bg-yellow-100 text-yellow-800';
      case 'danger':
        return 'bg-red-100 text-red-800';
      case 'info':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-primary-100 text-primary-800';
    }
  }
}
