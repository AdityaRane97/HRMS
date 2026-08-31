import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

// TODO: Create TasksService
@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">Tasks</h1>
      <!-- TODO: Implement task management and tracking -->
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-gray-600">Tasks will appear here.</p>
      </div>
    </div>
  `,
})
export class TasksComponent {}
