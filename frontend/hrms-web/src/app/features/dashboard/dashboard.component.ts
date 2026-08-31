import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import { DashboardService } from './dashboard.service';
import { EmployeeDashboard, ManagerDashboard, AdminDashboard } from './dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  employeeDashboard: EmployeeDashboard | null = null;
  managerDashboard: ManagerDashboard | null = null;
  adminDashboard: AdminDashboard | null = null;
  loading = true;
  error: string | null = null;

  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService
  ) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.currentUserRole = user.role;
      this.loadDashboardData();
    }
  }

  private loadDashboardData(): void {
    this.loading = true;
    this.error = null;

    switch (this.currentUserRole) {
      case UserRole.Employee:
        this.dashboardService.getEmployeeDashboard().subscribe({
          next: (data) => {
            this.employeeDashboard = data;
            this.loading = false;
          },
          error: (err) => {
            this.error = 'Failed to load employee dashboard';
            this.loading = false;
          },
        });
        break;

      case UserRole.Manager:
        this.dashboardService.getManagerDashboard().subscribe({
          next: (data) => {
            this.managerDashboard = data;
            this.loading = false;
          },
          error: (err) => {
            this.error = 'Failed to load manager dashboard';
            this.loading = false;
          },
        });
        break;

      case UserRole.Admin:
        this.dashboardService.getAdminDashboard().subscribe({
          next: (data) => {
            this.adminDashboard = data;
            this.loading = false;
          },
          error: (err) => {
            this.error = 'Failed to load admin dashboard';
            this.loading = false;
          },
        });
        break;

      default:
        this.error = 'Unknown user role';
        this.loading = false;
    }
  }
}
