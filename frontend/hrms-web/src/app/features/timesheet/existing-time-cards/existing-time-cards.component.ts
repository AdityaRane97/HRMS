import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface TimeCard {
  id: string;
  dateRange: string;
  totalHours: number;
  status: 'Approved' | 'Pending' | 'Rejected' | 'Draft';
  submittedDate: string;
}

@Component({
  selector: 'app-existing-time-cards',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './existing-time-cards.component.html',
  styleUrls: ['./existing-time-cards.component.scss'],
})
export class ExistingTimeCardsComponent {
  timeCards: TimeCard[] = [
    {
      id: '1',
      dateRange: 'Aug 18 - Aug 24, 2024',
      totalHours: 40,
      status: 'Approved',
      submittedDate: '2024-08-24',
    },
    {
      id: '2',
      dateRange: 'Aug 11 - Aug 17, 2024',
      totalHours: 40,
      status: 'Approved',
      submittedDate: '2024-08-17',
    },
    {
      id: '3',
      dateRange: 'Aug 04 - Aug 10, 2024',
      totalHours: 39.5,
      status: 'Approved',
      submittedDate: '2024-08-10',
    },
    {
      id: '4',
      dateRange: 'Jul 28 - Aug 03, 2024',
      totalHours: 40,
      status: 'Pending',
      submittedDate: '2024-08-03',
    },
    {
      id: '5',
      dateRange: 'Jul 21 - Jul 27, 2024',
      totalHours: 38.5,
      status: 'Approved',
      submittedDate: '2024-07-27',
    },
    {
      id: '6',
      dateRange: 'Jul 14 - Jul 20, 2024',
      totalHours: 40,
      status: 'Approved',
      submittedDate: '2024-07-20',
    },
  ];

  filteredTimeCards = [...this.timeCards];
  searchTerm = '';
  sortField: 'dateRange' | 'status' | 'totalHours' = 'dateRange';
  sortAscending = false;
  statusFilter: string = 'All';

  constructor(
    private location: Location,
    private router: Router
  ) {}

  goBack(): void {
    this.location.back();
  }

  onSearch(): void {
    this.applyFilters();
  }

  onStatusFilterChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    this.filteredTimeCards = this.timeCards.filter(card => {
      const matchesSearch = card.dateRange.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesStatus = this.statusFilter === 'All' || card.status === this.statusFilter;
      return matchesSearch && matchesStatus;
    });
    this.sortCards();
  }

  sortCards(): void {
    this.filteredTimeCards.sort((a, b) => {
      let aValue: any;
      let bValue: any;

      if (this.sortField === 'dateRange') {
        aValue = new Date(a.submittedDate).getTime();
        bValue = new Date(b.submittedDate).getTime();
      } else if (this.sortField === 'totalHours') {
        aValue = a.totalHours;
        bValue = b.totalHours;
      } else if (this.sortField === 'status') {
        aValue = a.status;
        bValue = b.status;
      }

      return this.sortAscending ? aValue - bValue : bValue - aValue;
    });
  }

  onSort(field: 'dateRange' | 'status' | 'totalHours'): void {
    if (this.sortField === field) {
      this.sortAscending = !this.sortAscending;
    } else {
      this.sortField = field;
      this.sortAscending = false;
    }
    this.sortCards();
  }

  viewTimeCard(id: string): void {
    // TODO: Navigate to time card detail view
    console.log('Viewing time card:', id);
  }

  downloadTimeCard(id: string): void {
    // TODO: Implement download functionality
    console.log('Downloading time card:', id);
  }

  getStatusColor(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Approved': 'bg-green-100 text-green-700',
      'Pending': 'bg-yellow-100 text-yellow-700',
      'Rejected': 'bg-red-100 text-red-700',
      'Draft': 'bg-gray-100 text-gray-700',
    };
    return statusMap[status] || 'bg-gray-100 text-gray-700';
  }

  formatDate(dateString: string): string {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-IN', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
      });
    } catch {
      return dateString;
    }
  }
}
