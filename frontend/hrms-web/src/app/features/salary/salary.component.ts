import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { QuickAccessCardComponent, QuickAccessItemData } from '@shared/components/quick-access-card.component';

@Component({
  selector: 'app-salary',
  standalone: true,
  imports: [CommonModule, RouterModule, QuickAccessCardComponent],
  templateUrl: './salary.component.html',
  styleUrl: './salary.component.scss',
})
export class SalaryComponent {
  salaryQuickAccess: QuickAccessItemData[] = [
    {
      title: 'Payslips',
      description: 'View and download your payslips',
      route: '/salary/payslip',
      iconEmoji: '📄',
    },
    {
      title: 'CTC Details',
      description: 'Annual compensation details',
      route: '/salary/payslip',
      iconEmoji: '💼',
    },
    {
      title: 'Reimbursement',
      description: 'Reimbursement payslips',
      route: '/salary/payslip',
      iconEmoji: '💰',
    },
    {
      title: 'YTD Reports',
      description: 'Year-to-date salary statements',
      route: '/salary/ytd-reports',
      iconEmoji: '📊',
    },
    {
      title: 'IT Declaration',
      description: 'Income tax declarations',
      route: '/salary/it-declaration',
      iconEmoji: '📋',
    },
    {
      title: 'FBP Declaration',
      description: 'Flexible benefit program',
      route: '/salary/fbp-declaration',
      iconEmoji: '🎁',
    },
  ];
}
