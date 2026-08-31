import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

// TODO: Create DocumentsService
@Component({
  selector: 'app-documents',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">Documents</h1>
      <!-- TODO: Implement document management and upload -->
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-gray-600">Documents will appear here.</p>
      </div>
    </div>
  `,
})
export class DocumentsComponent {}
