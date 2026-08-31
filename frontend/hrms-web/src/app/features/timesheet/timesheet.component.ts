import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

interface DashboardCard {
  title: string;
  description: string;
  icon: string;
  route: string;
  color: string;
}

@Component({
  selector: 'app-timesheet',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './timesheet.component.html',
  styleUrls: ['./timesheet.component.scss'],
})
export class TimesheetComponent {
  dashboardCards: DashboardCard[] = [
    {
      title: 'Current Time Card',
      description: 'Open your current time card',
      icon: 'current-card',
      route: '/timesheet/current-time-card',
      color: 'from-blue-500 to-blue-600',
    },
    {
      title: 'Existing Time Cards',
      description: 'Access all your time cards',
      icon: 'existing-cards',
      route: '/timesheet/existing-time-cards',
      color: 'from-green-500 to-green-600',
    },
    {
      title: 'Team Schedule',
      description: "View your team's shifts and absences",
      icon: 'team-schedule',
      route: '/timesheet/team-schedule',
      color: 'from-purple-500 to-purple-600',
    },
    {
      title: 'Add Absence',
      description: 'Request an absence',
      icon: 'add-absence',
      route: '/timesheet/add-absence',
      color: 'from-orange-500 to-orange-600',
    },
    {
      title: 'Absence Balance',
      description: 'Review current plan balances',
      icon: 'absence-balance',
      route: '/timesheet/absence-balance',
      color: 'from-pink-500 to-pink-600',
    },
    {
      title: 'Existing Absences',
      description: 'View and manage absence requests',
      icon: 'existing-absences',
      route: '/timesheet/existing-absences',
      color: 'from-red-500 to-red-600',
    },
  ];

  constructor(private router: Router) {}

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  getIconSvg(iconType: string): string {
    const icons: { [key: string]: string } = {
      'current-card': `<svg class="w-12 h-12" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>`,
      'existing-cards': `<svg class="w-12 h-12" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
      </svg>`,
      'team-schedule': `<svg class="w-12 h-12" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 4.354a4 4 0 110 5.292M15 21H3v-2a6 6 0 0112 0v2zm0 0h6v-2a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
      </svg>`,
      'add-absence': `<svg class="w-12 h-12" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 4v16m8-8H4" />
      </svg>`,
      'absence-balance': `<svg class="w-12 h-12" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2z" />
      </svg>`,
      'existing-absences': `<svg class="w-12 h-12" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
      </svg>`,
    };

    return icons[iconType] || '';
  }
}
