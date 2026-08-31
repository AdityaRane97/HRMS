import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

export interface QuickAccessItemData {
  title: string;
  description?: string;
  icon?: string;
  route: string;
  iconEmoji?: string;
  color?: string;
}

@Component({
  selector: 'app-quick-access-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <a
      [routerLink]="item.route"
      class="bg-white rounded-lg shadow p-6 hover:shadow-lg transition-all hover:scale-105 block group cursor-pointer"
    >
      <div class="text-4xl mb-4" *ngIf="item.iconEmoji">{{ item.iconEmoji }}</div>
      <h3 class="font-semibold text-gray-900 mb-2 group-hover:text-primary-600 transition-colors">
        {{ item.title }}
      </h3>
      <p *ngIf="item.description" class="text-sm text-gray-600 group-hover:text-gray-700 transition-colors">
        {{ item.description }}
      </p>
    </a>
  `,
  styles: [],
})
export class QuickAccessCardComponent {
  @Input() item!: QuickAccessItemData;
}
