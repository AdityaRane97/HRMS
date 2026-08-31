import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

// TODO: Create SalaryService
@Component({
  selector: 'app-salary',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">Salary</h1>
      <!-- TODO: Implement salary details view -->
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-gray-600">Salary information and history will appear here.</p>
      </div>
    </div>
  `,
})
export class SalaryComponent {}
