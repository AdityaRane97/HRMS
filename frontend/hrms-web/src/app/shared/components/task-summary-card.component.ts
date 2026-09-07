import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface TaskSummaryCardData {
  title: string;
  count: number;
  icon: string;
  color: 'blue' | 'green' | 'yellow' | 'red' | 'purple';
  trend?: number; // percentage change
}

@Component({
  selector: 'app-task-summary-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div [ngClass]="'bg-white rounded-lg shadow p-6 border-l-4 ' + getBorderColor()">
      <div class="flex justify-between items-start">
        <div>
          <p class="text-gray-600 text-sm mb-1">{{ card.title }}</p>
          <h3 class="text-3xl font-bold text-gray-900">{{ card.count }}</h3>
          <div *ngIf="card.trend !== undefined" class="mt-2 text-xs">
            <span [ngClass]="card.trend >= 0 ? 'text-green-600' : 'text-red-600'">
              {{ card.trend >= 0 ? '↑' : '↓' }} {{ Math.abs(card.trend) }}%
            </span>
            <span class="text-gray-500 ml-1">vs last period</span>
          </div>
        </div>
        <span class="text-4xl">{{ card.icon }}</span>
      </div>
    </div>
  `,
})
export class TaskSummaryCardComponent {
  @Input() card!: TaskSummaryCardData;

  Math = Math;

  getBorderColor(): string {
    const colorMap: Record<string, string> = {
      blue: 'border-blue-500',
      green: 'border-green-500',
      yellow: 'border-yellow-500',
      red: 'border-red-500',
      purple: 'border-purple-500',
    };
    return colorMap[this.card.color] || 'border-gray-500';
  }
}
