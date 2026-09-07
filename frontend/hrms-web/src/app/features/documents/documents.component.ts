import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import { EmployeeDocumentsComponent } from './employee-documents/employee-documents.component';
import { AdminDocumentsComponent } from './admin-documents/admin-documents.component';

@Component({
  selector: 'app-documents',
  standalone: true,
  imports: [CommonModule, EmployeeDocumentsComponent, AdminDocumentsComponent],
  template: `
    <div>
      <!-- Employee View -->
      <app-employee-documents
        *ngIf="currentUserRole === UserRole.Employee"
      ></app-employee-documents>

      <!-- Admin/Manager View -->
      <app-admin-documents
        *ngIf="
          currentUserRole === UserRole.Admin || currentUserRole === UserRole.Manager
        "
      ></app-admin-documents>

      <!-- Fallback for other roles -->
      <div *ngIf="!currentUserRole" class="p-6">
        <p class="text-gray-600">Unable to loading documents.</p>
      </div>
    </div>
  `,
})
export class DocumentsComponent implements OnInit {
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.currentUserRole = user.role;
    }
  }
}
