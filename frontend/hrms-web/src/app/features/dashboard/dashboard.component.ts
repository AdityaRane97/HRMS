import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '@core/auth/auth.service';
import { UserRole } from '@core/models/role.enum';
import { DashboardService } from './dashboard.service';
import { EmployeeDashboard, ManagerDashboard, AdminDashboard } from './dashboard.model';
import { SalaryService } from '@features/salary/salary.service';
import { SalarySummary } from '@features/salary/salary.model';
import { DashboardCardComponent, DashboardCardData } from '@shared/components/dashboard-card.component';
import { StatusCardComponent, StatusCardData } from '@shared/components/status-card.component';
import { QuickAccessCardComponent, QuickAccessItemData } from '@shared/components/quick-access-card.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    DashboardCardComponent,
    StatusCardComponent,
    QuickAccessCardComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  UserRole = UserRole;
  currentUserRole: UserRole | null = null;
  currentUserName: string = '';
  employeeDashboard: EmployeeDashboard | null = null;
  managerDashboard: ManagerDashboard | null = null;
  adminDashboard: AdminDashboard | null = null;
  salarySummary: SalarySummary | null = null;

  loading = true;
  error: string | null = null;

  // Dashboard cards data
  summaryCards: DashboardCardData[] = [];
  statusCards: StatusCardData[] = [];
  quickAccessItems: QuickAccessItemData[] = [];

  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService,
    private salaryService: SalaryService
  ) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.currentUserRole = user.role;
      this.currentUserName = user.firstName || 'Employee';
      this.loadDashboardData();
      this.loadSalarySummary();
      this.initializeQuickAccess();
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

  private loadSalarySummary(): void {
    this.salaryService.getSalarySummary().subscribe({
      next: (data) => {
        this.salarySummary = data;
        this.initializeSummaryCards();
        this.initializeStatusCards();
      },
      error: () => {
        // Continue even if salary data fails
      },
    });
  }

  private initializeSummaryCards(): void {
    if (this.salarySummary) {
      this.summaryCards = [
        {
          title: 'Current Month Net Pay',
          value: `₹ ${this.salarySummary.netPayCurrentMonth.toLocaleString()}`,
          subtitle: this.salarySummary.currentMonth,
          color: 'success',
        },
        {
          title: 'YTD Net Pay',
          value: `₹ ${this.salarySummary.netPayYTD.toLocaleString()}`,
          subtitle: `Year to Date`,
          color: 'info',
        },
        {
          title: 'Annual CTC',
          value: `₹ ${this.salarySummary.ctcAmount.toLocaleString()}`,
          subtitle: `2024-2025`,
          color: 'primary',
        },
        {
          title: 'Pending Payslips',
          value: this.salarySummary.pendingPayslips,
          badge: 'Pending',
          color: 'warning',
        },
      ];
    }
  }

  private initializeStatusCards(): void {
    this.statusCards = [
      {
        title: 'Attendance',
        status: 'active',
        value: '22 Days',
        details: ['Leaves taken: 2', 'Present: 22', 'Absent: 0'],
      },
      {
        title: 'FBP Declaration',
        status: 'completed',
        value: 'Submitted',
        details: ['Total Declared: ₹ 3,05,000', 'Status: Verified'],
      },
      {
        title: 'IT Declaration',
        status: 'completed',
        value: 'Submitted',
        details: ['80C: ₹ 1,50,000', 'Status: Verified'],
      },
      {
        title: 'Reimbursement',
        status: 'pending',
        value: '₹ 5,000',
        details: ['Pending Approval', 'Last Updated: Today'],
      },
    ];
  }

  private initializeQuickAccess(): void {
    this.quickAccessItems = [
      {
        title: 'My Profile',
        description: 'View and update your profile',
        route: '/profile',
        iconEmoji: '👤',
      },
      {
        title: 'Timesheet',
        description: 'Track your time and attendance',
        route: '/timesheet',
        iconEmoji: '⏱',
      },
      {
        title: 'Absence',
        description: 'Request and manage absences',
        route: '/timesheet/add-absence',
        iconEmoji: '📋',
      },
      {
        title: 'Payslips',
        description: 'Download your payslips',
        route: '/salary/payslip',
        iconEmoji: '📄',
      },
      {
        title: 'IT Declaration',
        description: 'Manage tax declarations',
        route: '/salary/it-declaration',
        iconEmoji: '📊',
      },
      {
        title: 'FBP Benefits',
        description: 'Flexible benefit declarations',
        route: '/salary/fbp-declaration',
        iconEmoji: '🎁',
      },
      {
        title: 'YTD Reports',
        description: 'Year-to-date salary reports',
        route: '/salary/ytd-reports',
        iconEmoji: '📈',
      },
      {
        title: 'CTC Details',
        description: 'Annual salary structure',
        route: '/salary/payslip',
        iconEmoji: '💼',
      },
    ];
  }
}
