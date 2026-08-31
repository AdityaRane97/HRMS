import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

// TODO: Create PoliciesService
@Component({
  selector: 'app-policies',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">Company Policies</h1>
      <!-- TODO: Implement policy browsing and acknowledgment -->
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-gray-600">Company policies will appear here.</p>
      </div>
    </div>
  `,
})
export class PoliciesComponent {}
