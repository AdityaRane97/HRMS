import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Location } from '@angular/common';

interface TeamMember {
  id: string;
  name: string;
  designation: string;
  status: 'Present' | 'Absent' | 'On Leave' | 'WFH';
  shift: string;
  date: string;
}

@Component({
  selector: 'app-team-schedule',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './team-schedule.component.html',
  styleUrls: ['./team-schedule.component.scss'],
})
export class TeamScheduleComponent {
  teamMembers: TeamMember[] = [
    {
      id: '1',
      name: 'Aditya Rane',
      designation: 'Senior Software Engineer',
      status: 'Present',
      shift: '09:00 - 17:00',
      date: '2024-08-31',
    },
    {
      id: '2',
      name: 'Jane Smith',
      designation: 'Product Manager',
      status: 'On Leave',
      shift: '09:00 - 17:00',
      date: '2024-08-31',
    },
    {
      id: '3',
      name: 'John Manager',
      designation: 'Engineering Lead',
      status: 'Present',
      shift: '09:00 - 18:00',
      date: '2024-08-31',
    },
    {
      id: '4',
      name: 'Sarah Johnson',
      designation: 'QA Engineer',
      status: 'WFH',
      shift: '09:00 - 17:00',
      date: '2024-08-31',
    },
    {
      id: '5',
      name: 'Mike Chen',
      designation: 'DevOps Engineer',
      status: 'Present',
      shift: '08:00 - 16:00',
      date: '2024-08-31',
    },
    {
      id: '6',
      name: 'Emily Brown',
      designation: 'UI/UX Designer',
      status: 'Absent',
      shift: '09:00 - 17:00',
      date: '2024-08-31',
    },
    {
      id: '7',
      name: 'Alex Kumar',
      designation: 'Backend Developer',
      status: 'Present',
      shift: '09:00 - 17:00',
      date: '2024-08-31',
    },
    {
      id: '8',
      name: 'Lisa White',
      designation: 'Frontend Developer',
      status: 'On Leave',
      shift: '09:00 - 17:00',
      date: '2024-08-31',
    },
  ];

  selectedDate = '2024-08-31';

  constructor(private location: Location) {}

  goBack(): void {
    this.location.back();
  }

  getStatusColor(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Present': 'bg-green-100 text-green-700',
      'Absent': 'bg-red-100 text-red-700',
      'On Leave': 'bg-orange-100 text-orange-700',
      'WFH': 'bg-blue-100 text-blue-700',
    };
    return statusMap[status] || 'bg-gray-100 text-gray-700';
  }

  getStatusIcon(status: string): string {
    const iconMap: { [key: string]: string } = {
      'Present': '✓',
      'Absent': '✗',
      'On Leave': '📅',
      'WFH': '💻',
    };
    return iconMap[status] || '•';
  }

  getPresentCount(): number {
    return this.teamMembers.filter(m => m.status === 'Present').length;
  }

  getAbsentCount(): number {
    return this.teamMembers.filter(m => m.status === 'Absent').length;
  }

  getOnLeaveCount(): number {
    return this.teamMembers.filter(m => m.status === 'On Leave').length;
  }

  formatDate(dateString: string): string {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-IN', {
        weekday: 'long',
        month: 'long',
        day: 'numeric',
        year: 'numeric',
      });
    } catch {
      return dateString;
    }
  }
}
