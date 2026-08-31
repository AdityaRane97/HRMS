import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Location } from '@angular/common';

interface TimeEntry {
  id: string;
  date: string;
  projectClient: string;
  activity: string;
  description: string;
  hours: number;
}

@Component({
  selector: 'app-current-time-card',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './current-time-card.component.html',
  styleUrls: ['./current-time-card.component.scss'],
})
export class CurrentTimeCardComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  saving = false;
  message: string | null = null;
  editing = false;

  // Mock employee data
  employeeName = 'Aditya Rane';
  employeeInitials = 'AR';
  timeCardDateRange = 'Week of Aug 25 - Aug 31, 2024';
  status = 'In Progress';
  totalHours = 40;

  // Mock time entries
  timeEntries: TimeEntry[] = [
    {
      id: '1',
      date: '2024-08-25',
      projectClient: 'Project Alpha',
      activity: 'Development',
      description: 'API endpoint implementation',
      hours: 8.0,
    },
    {
      id: '2',
      date: '2024-08-26',
      projectClient: 'Project Alpha',
      activity: 'Code Review',
      description: 'Reviewed PR #234',
      hours: 8.0,
    },
    {
      id: '3',
      date: '2024-08-27',
      projectClient: 'Project Beta',
      activity: 'Development',
      description: 'Database optimization',
      hours: 8.0,
    },
    {
      id: '4',
      date: '2024-08-28',
      projectClient: 'Project Alpha',
      activity: 'Testing',
      description: 'Unit test coverage',
      hours: 8.0,
    },
    {
      id: '5',
      date: '2024-08-29',
      projectClient: 'Project Beta',
      activity: 'Documentation',
      description: 'API documentation',
      hours: 8.0,
    },
  ];

  // Daily breakdown (mock data)
  dailyBreakdown = [
    { day: 'Monday', hours: 8.0 },
    { day: 'Tuesday', hours: 8.0 },
    { day: 'Wednesday', hours: 8.0 },
    { day: 'Thursday', hours: 8.0 },
    { day: 'Friday', hours: 8.0 },
    { day: 'Saturday', hours: 0 },
    { day: 'Sunday', hours: 0 },
  ];

  constructor(
    private formBuilder: FormBuilder,
    private location: Location,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.form = this.formBuilder.group({
      date: ['', Validators.required],
      project: ['', Validators.required],
      activity: ['', Validators.required],
      description: [''],
      hours: ['', [Validators.required, Validators.min(0), Validators.max(24)]],
    });
  }

  goBack(): void {
    this.location.back();
  }

  toggleEdit(): void {
    this.editing = !this.editing;
  }

  addEntry(): void {
    this.editing = true;
    this.form.reset();
  }

  editEntry(entry: TimeEntry): void {
    this.form.patchValue({
      date: entry.date,
      project: entry.projectClient,
      activity: entry.activity,
      description: entry.description,
      hours: entry.hours,
    });
    this.editing = true;
  }

  deleteEntry(id: string): void {
    this.timeEntries = this.timeEntries.filter(e => e.id !== id);
    this.message = 'Entry deleted';
    setTimeout(() => (this.message = null), 2000);
  }

  onSave(): void {
    if (this.form.invalid) return;
    this.saving = true;
    setTimeout(() => {
      this.saving = false;
      this.message = 'Time card saved successfully';
      this.editing = false;
      setTimeout(() => (this.message = null), 2000);
    }, 500);
  }

  onSubmit(): void {
    this.saving = true;
    setTimeout(() => {
      this.saving = false;
      this.message = 'Time card submitted successfully';
      this.status = 'Submitted';
      setTimeout(() => (this.message = null), 2000);
    }, 500);
  }

  onDone(): void {
    this.router.navigate(['/timesheet']);
  }

  getStatusColor(): string {
    const statusMap: { [key: string]: string } = {
      'In Progress': 'bg-blue-100 text-blue-700',
      'Submitted': 'bg-green-100 text-green-700',
      'Approved': 'bg-green-100 text-green-700',
      'Rejected': 'bg-red-100 text-red-700',
    };
    return statusMap[this.status] || 'bg-gray-100 text-gray-700';
  }

  formatDate(dateString: string): string {
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-IN', {
        weekday: 'short',
        month: 'short',
        day: 'numeric',
      });
    } catch {
      return dateString;
    }
  }
}
