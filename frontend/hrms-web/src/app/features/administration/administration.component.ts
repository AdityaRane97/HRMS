import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

// TODO: Create AdministrationService
@Component({
  selector: 'app-administration',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">Administration</h1>
      <!-- TODO: Implement admin panel for system management -->
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-gray-600">Admin tools and settings will appear here.</p>
      </div>
    </div>
  `,
})
export class AdministrationComponent {}
