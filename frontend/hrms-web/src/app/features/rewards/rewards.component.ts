import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

// TODO: Create RewardsService
@Component({
  selector: 'app-rewards',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">Rewards & Recognition</h1>
      <!-- TODO: Implement rewards tracking and recognition program -->
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-gray-600">Rewards and recognition will appear here.</p>
      </div>
    </div>
  `,
})
export class RewardsComponent {}
