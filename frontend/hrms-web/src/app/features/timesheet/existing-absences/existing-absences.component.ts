import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Absence {
  id: string;
  type: string;
  duration: string;
  startDate: string;
  endDate: string;
  status: 'Awaiting Approval' | 'Scheduled' | 'Completed' | 'Withdrawn' | 'Rejected';
  approverName?: string;
}

@Component({
  selector: 'app-existing-absences',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './existing-absences.component.html',
  styleUrls: ['./existing-absences.component.scss'],
})
export class ExistingAbsencesComponent {
  absences: Absence[] = [
    {
      id: '1',
      type: 'PTO',
      duration: '5 days',
      startDate: '2024-09-01',
      endDate: '2024-09-05',
      status: 'Scheduled',
      approverName: 'John Manager',
    },
    {
      id: '2',
      type: 'Sick Leave',
      duration: '1 day',
      startDate: '2024-08-28',
      endDate: '2024-08-28',
      status: 'Completed',
      approverName: 'Jane Lead',
    },
    {
      id: '3',
      type: 'Compensatory Off',
      duration: '2 days',
      startDate: '2024-08-15',
      endDate: '2024-08-16',
      status: 'Completed',
      approverName: 'John Manager',
    },
    {
      id: '4',
      type: 'PTO',
      duration: '3 days',
      startDate: '2024-09-10',
      endDate: '2024-09-12',
      status: 'Awaiting Approval',
    },
    {
      id: '5',
      type: 'Bereavement Leave',
      duration: '2 days',
      startDate: '2024-08-20',
      endDate: '2024-08-21',
      status: 'Completed',
      approverName: 'John Manager',
    },
    {
      id: '6',
      type: 'PTO',
      duration: '1 day',
      startDate: '2024-08-10',
      endDate: '2024-08-10',
      status: 'Withdrawn',
    },
  ];

  filteredAbsences = [...this.absences];
  searchTerm = '';
  sortField: 'startDate' | 'status' | 'type' = 'startDate';
  sortAscending = false;
  statusFilter: string = 'All';

  constructor(
    private location: Location,
    private router: Router
  ) {}

  goBack(): void {
    this.location.back();
  }

  addAbsence(): void {
    this.router.navigate(['/timesheet/add-absence']);
  }

  onSearch(): void {
    this.applyFilters();
  }

  onStatusFilterChange(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    this.filteredAbsences = this.absences.filter(absence => {
      const matchesSearch = absence.type.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        absence.duration.toLowerCase().includes(this.searchTerm.toLowerCase());
      const matchesStatus = this.statusFilter === 'All' || absence.status === this.statusFilter;
      return matchesSearch && matchesStatus;
    });
    this.sortAbsences();
  }

  sortAbsences(): void {
    this.filteredAbsences.sort((a, b) => {
      let aValue: any;
      let bValue: any;

      if (this.sortField === 'startDate') {
        aValue = new Date(a.startDate).getTime();
        bValue = new Date(b.startDate).getTime();
      } else if (this.sortField === 'type') {
        aValue = a.type;
        bValue = b.type;
      } else if (this.sortField === 'status') {
        aValue = a.status;
        bValue = b.status;
      }

      return this.sortAscending ? aValue - bValue : bValue - aValue;
    });
  }

  onSort(field: 'startDate' | 'status' | 'type'): void {
    if (this.sortField === field) {
      this.sortAscending = !this.sortAscending;
    } else {
      this.sortField = field;
      this.sortAscending = false;
    }
    this.sortAbsences();
  }

  editAbsence(id: string): void {
    // TODO: Navigate to edit absence page
    console.log('Editing absence:', id);
  }

  withdrawAbsence(id: string): void {
    const absence = this.absences.find(a => a.id === id);
    if (absence && absence.status !== 'Completed') {
      absence.status = 'Withdrawn';
      this.applyFilters();
    }
  }

  getStatusColor(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Awaiting Approval': 'bg-yellow-100 text-yellow-700',
      'Scheduled': 'bg-blue-100 text-blue-700',
      'Completed': 'bg-green-100 text-green-700',
      'Withdrawn': 'bg-gray-100 text-gray-700',
      'Rejected': 'bg-red-100 text-red-700',
    };
    return statusMap[status] || 'bg-gray-100 text-gray-700';
  }

  formatDate(dateString: string): string {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-IN', {
        month: 'short',
        day: 'numeric',
        year: '2-digit',
      });
    } catch {
      return dateString;
    }
  }
}
