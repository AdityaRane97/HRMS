import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Location } from '@angular/common';

@Component({
  selector: 'app-add-absence',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-absence.component.html',
  styleUrls: ['./add-absence.component.scss'],
})
export class AddAbsenceComponent implements OnInit {
  form!: FormGroup;
  loading = false;
  saving = false;
  message: string | null = null;

  employeeName = 'Aditya Rane';
  employeeInitials = 'AR';

  absenceTypes = [
    { value: 'pto', label: 'PTO (Paid Time Off)' },
    { value: 'sick', label: 'Sick Leave' },
    { value: 'compensatory', label: 'Compensatory Off' },
    { value: 'paternity', label: 'Paternity Leave' },
    { value: 'bereavement', label: 'Bereavement Leave' },
    { value: 'unpaid', label: 'Unpaid Leave' },
  ];

  absenceBalances: { [key: string]: { balance: number; unit: string } } = {
    pto: { balance: 13.5, unit: 'Days' },
    sick: { balance: 8, unit: 'Days' },
    compensatory: { balance: 0, unit: 'Hours' },
    paternity: { balance: 15, unit: 'Days' },
    bereavement: { balance: 3, unit: 'Days' },
    unpaid: { balance: 999, unit: 'Days' },
  };

  showDetails = false;
  showComments = false;
  calculatedDuration = 0;

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
      absenceType: ['pto', Validators.required],
      startDate: ['', Validators.required],
      startTime: ['09:00'],
      endDate: ['', Validators.required],
      endTime: ['17:00'],
      reason: [''],
      comments: [''],
      attachments: [''],
    });

    // Watch for date changes to calculate duration
    this.form.get('startDate')?.valueChanges.subscribe(() => this.calculateDuration());
    this.form.get('endDate')?.valueChanges.subscribe(() => this.calculateDuration());
  }

  calculateDuration(): void {
    const startDate = this.form.get('startDate')?.value;
    const endDate = this.form.get('endDate')?.value;

    if (startDate && endDate) {
      const start = new Date(startDate);
      const end = new Date(endDate);
      const diffMs = end.getTime() - start.getTime();
      const diffDays = Math.ceil(diffMs / (1000 * 60 * 60 * 24)) + 1;
      this.calculatedDuration = diffDays > 0 ? diffDays : 0;
    }
  }

  toggleDetails(): void {
    this.showDetails = !this.showDetails;
  }

  toggleComments(): void {
    this.showComments = !this.showComments;
  }

  getCurrentBalance(): { balance: number; unit: string } {
    const selectedType = this.form.get('absenceType')?.value;
    return this.absenceBalances[selectedType] || { balance: 0, unit: 'Days' };
  }

  onSaveAndClose(): void {
    if (this.form.invalid) return;
    this.saving = true;
    setTimeout(() => {
      this.saving = false;
      this.message = 'Absence saved successfully';
      setTimeout(() => this.router.navigate(['/timesheet']), 1500);
    }, 500);
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.saving = true;
    setTimeout(() => {
      this.saving = false;
      this.message = 'Absence submitted for approval';
      setTimeout(() => this.router.navigate(['/timesheet']), 1500);
    }, 500);
  }

  onCancel(): void {
    this.location.back();
  }

  goBack(): void {
    this.location.back();
  }

  formatDate(dateString: string): string {
    if (!dateString) return '-';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-IN', {
        weekday: 'short',
        month: 'short',
        day: 'numeric',
        year: 'numeric',
      });
    } catch {
      return dateString;
    }
  }
}
