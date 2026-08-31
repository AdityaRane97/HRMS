import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

// TODO: Create ChatbotService
@Component({
  selector: 'app-chatbot',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="p-6">
      <h1 class="text-3xl font-bold text-gray-900 mb-6">HR Chatbot</h1>
      <!-- TODO: Implement chatbot interface for HR queries -->
      <div class="bg-white rounded-lg shadow p-6">
        <p class="text-gray-600">Chat interface will appear here.</p>
      </div>
    </div>
  `,
})
export class ChatbotComponent {}
