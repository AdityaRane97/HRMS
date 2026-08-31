import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface StatusCardData {
  title: string;
  status: 'active' | 'inactive' | 'pending' | 'completed' | 'warning';
  value?: string;
  details?: string[];
  action?: string;
}

@Component({
  selector: 'app-status-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [ngClass]="'rounded-lg shadow p-6 border-l-4 ' + getBackgroundClass() + ' ' + getBorderClass()">
      <div class="flex justify-between items-start">
        <div class="flex-1">
          <h3 class="font-semibold text-gray-900 mb-2">{{ card.title }}</h3>
          <p [ngClass]="getStatusBadgeClass()" class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium mb-3">
            {{ card.status | uppercase }}
          </p>
          <p *ngIf="card.value" class="text-2xl font-bold text-gray-900 mb-2">{{ card.value }}</p>
          <ul *ngIf="card.details" class="space-y-1">
            <li *ngFor="let detail of card.details" class="text-sm text-gray-700 flex items-center gap-2">
              <span class="w-1 h-1 bg-gray-400 rounded-full"></span>
              {{ detail }}
            </li>
          </ul>
        </div>
        <span [ngClass]="getStatusIconClass()" class="text-2xl">
          {{ getStatusIcon() }}
        </span>
      </div>
    </div>
  `,
  styles: [],
})
export class StatusCardComponent {
  @Input() card!: StatusCardData;

  getBackgroundClass(): string {
    switch (this.card.status) {
      case 'completed':
        return 'bg-green-50';
      case 'active':
        return 'bg-blue-50';
      case 'pending':
        return 'bg-yellow-50';
      case 'warning':
        return 'bg-red-50';
      default:
        return 'bg-gray-50';
    }
  }

  getBorderClass(): string {
    switch (this.card.status) {
      case 'completed':
        return 'border-green-500';
      case 'active':
        return 'border-blue-500';
      case 'pending':
        return 'border-yellow-500';
      case 'warning':
        return 'border-red-500';
      default:
        return 'border-gray-300';
    }
  }

  getStatusBadgeClass(): string {
    switch (this.card.status) {
      case 'completed':
        return 'bg-green-100 text-green-800';
      case 'active':
        return 'bg-blue-100 text-blue-800';
      case 'pending':
        return 'bg-yellow-100 text-yellow-800';
      case 'warning':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  getStatusIconClass(): string {
    switch (this.card.status) {
      case 'completed':
        return 'text-green-600';
      case 'active':
        return 'text-blue-600';
      case 'pending':
        return 'text-yellow-600';
      case 'warning':
        return 'text-red-600';
      default:
        return 'text-gray-600';
    }
  }

  getStatusIcon(): string {
    switch (this.card.status) {
      case 'completed':
        return '✓';
      case 'active':
        return '●';
      case 'pending':
        return '⏱';
      case 'warning':
        return '⚠';
      default:
        return '●';
    }
  }
}
