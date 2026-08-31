import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Location } from '@angular/common';

interface AbsencePlan {
  name: string;
  balance: number;
  unit: string;
  used: number;
  total: number;
  color: string;
}

@Component({
  selector: 'app-absence-balance',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './absence-balance.component.html',
  styleUrls: ['./absence-balance.component.scss'],
})
export class AbsenceBalanceComponent {
  balanceAsOfDate = '2024-08-31';

  plans: AbsencePlan[] = [
    {
      name: 'Paid Time Off (PTO)',
      balance: 13.5,
      unit: 'Days',
      used: 6.5,
      total: 20,
      color: 'blue',
    },
    {
      name: 'Sick Leave',
      balance: 8,
      unit: 'Days',
      used: 2,
      total: 10,
      color: 'green',
    },
    {
      name: 'Compensatory Off',
      balance: 0,
      unit: 'Hours',
      used: 16,
      total: 16,
      color: 'orange',
    },
    {
      name: 'Paternity Leave',
      balance: 15,
      unit: 'Days',
      used: 0,
      total: 15,
      color: 'purple',
    },
    {
      name: 'Bereavement Leave',
      balance: 3,
      unit: 'Days',
      used: 0,
      total: 3,
      color: 'red',
    },
    {
      name: 'Unpaid Leave',
      balance: 999,
      unit: 'Days',
      used: 0,
      total: 999,
      color: 'gray',
    },
  ];

  constructor(private location: Location) {}

  goBack(): void {
    this.location.back();
  }

  getProgressBarColor(color: string): string {
    const colorMap: { [key: string]: string } = {
      'blue': 'bg-blue-500',
      'green': 'bg-green-500',
      'orange': 'bg-orange-500',
      'purple': 'bg-purple-500',
      'red': 'bg-red-500',
      'gray': 'bg-gray-500',
    };
    return colorMap[color] || 'bg-blue-500';
  }

  getCardBorderColor(color: string): string {
    const colorMap: { [key: string]: string } = {
      'blue': 'border-l-blue-500',
      'green': 'border-l-green-500',
      'orange': 'border-l-orange-500',
      'purple': 'border-l-purple-500',
      'red': 'border-l-red-500',
      'gray': 'border-l-gray-500',
    };
    return colorMap[color] || 'border-l-blue-500';
  }

  getProgressPercentage(plan: AbsencePlan): number {
    if (plan.total === 0) return 0;
    return (plan.used / plan.total) * 100;
  }

  formatDate(dateString: string): string {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-IN', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      });
    } catch {
      return dateString;
    }
  }
}
