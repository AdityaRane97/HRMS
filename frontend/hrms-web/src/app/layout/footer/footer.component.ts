import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  standalone: true,
  template: `
    <footer class="bg-white border-t border-gray-200 py-4 px-6 text-center text-sm text-gray-500">
      <p>© 2024 HRMS. All rights reserved.</p>
    </footer>
  `,
})
export class FooterComponent {}
