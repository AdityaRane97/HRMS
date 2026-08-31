import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EmployeeDashboard, ManagerDashboard, AdminDashboard } from './dashboard.model';
import { API_CONFIG } from '@core/config/api.config';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(private http: HttpClient) {}

  getEmployeeDashboard(): Observable<EmployeeDashboard> {
    // TODO: Implement backend call
    return new Observable((observer) => {
      observer.next({
        widgets: [
          { id: '1', title: 'Profile Summary', type: 'summary' },
          { id: '2', title: 'Leave Balance', type: 'summary' },
          { id: '3', title: 'My Tasks', type: 'list' },
        ],
      });
      observer.complete();
    });
  }

  getManagerDashboard(): Observable<ManagerDashboard> {
    // TODO: Implement backend call
    return new Observable((observer) => {
      observer.next({
        widgets: [
          { id: '1', title: 'Team Summary', type: 'summary' },
          { id: '2', title: 'Team Performance', type: 'chart' },
          { id: '3', title: 'Pending Approvals', type: 'pending-approvals' },
        ],
      });
      observer.complete();
    });
  }

  getAdminDashboard(): Observable<AdminDashboard> {
    // TODO: Implement backend call
    return new Observable((observer) => {
      observer.next({
        widgets: [
          { id: '1', title: 'Employee Count', type: 'summary' },
          { id: '2', title: 'System Overview', type: 'chart' },
          { id: '3', title: 'Compliance Status', type: 'list' },
        ],
      });
      observer.complete();
    });
  }
}
