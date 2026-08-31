export interface DashboardWidget {
  id: string;
  title: string;
  type: 'summary' | 'chart' | 'list' | 'pending-approvals';
  data?: any;
}

export interface EmployeeDashboard {
  widgets: DashboardWidget[];
}

export interface ManagerDashboard {
  widgets: DashboardWidget[];
}

export interface AdminDashboard {
  widgets: DashboardWidget[];
}
