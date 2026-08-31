import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SalaryService } from '../salary.service';
import { Payslip, CTCPayslip, ReimbursementPayslip } from '../salary.model';

@Component({
  selector: 'app-payslip',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './payslip.component.html',
  styleUrls: ['./payslip.component.scss'],
})
export class PayslipComponent implements OnInit {
  activeTab: 'payslip' | 'ctc' | 'reimbursement' = 'payslip';

  // Payslip data
  payslip: Payslip | null = null;
  selectedMonth = 'January';
  selectedYear = 2024;
  months = ['January', 'February', 'March', 'April', 'May', 'June', 
            'July', 'August', 'September', 'October', 'November', 'December'];
  years = [2023, 2024, 2025];

  // CTC data
  ctcPayslip: CTCPayslip | null = null;
  selectedCTCYear = '2024-2025';
  ctcYears = ['2024-2025', '2023-2024', '2022-2023'];

  // Reimbursement data
  reimbursementPayslip: ReimbursementPayslip | null = null;
  selectedRembMonth = 'January';
  selectedRembYear = 2024;

  loading = false;
  error: string | null = null;

  constructor(private salaryService: SalaryService) {}

  ngOnInit(): void {
    this.loadPayslip();
  }

  loadPayslip(): void {
    this.loading = true;
    this.error = null;
    this.salaryService.getPayslip(this.selectedMonth, this.selectedYear).subscribe({
      next: (data) => {
        this.payslip = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load payslip';
        this.loading = false;
      },
    });
  }

  loadCTCPayslip(): void {
    this.loading = true;
    this.error = null;
    this.salaryService.getCTCPayslip(this.selectedCTCYear).subscribe({
      next: (data) => {
        this.ctcPayslip = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load CTC payslip';
        this.loading = false;
      },
    });
  }

  loadReimbursementPayslip(): void {
    this.loading = true;
    this.error = null;
    this.salaryService.getReimbursementPayslip(this.selectedRembMonth, this.selectedRembYear).subscribe({
      next: (data) => {
        this.reimbursementPayslip = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load reimbursement payslip';
        this.loading = false;
      },
    });
  }

  selectTab(tab: 'payslip' | 'ctc' | 'reimbursement'): void {
    this.activeTab = tab;
    if (tab === 'payslip' && !this.payslip) {
      this.loadPayslip();
    } else if (tab === 'ctc' && !this.ctcPayslip) {
      this.loadCTCPayslip();
    } else if (tab === 'reimbursement' && !this.reimbursementPayslip) {
      this.loadReimbursementPayslip();
    }
  }

  onMonthChange(): void {
    this.loadPayslip();
  }

  onCTCYearChange(): void {
    this.loadCTCPayslip();
  }

  onReimbursementMonthChange(): void {
    this.loadReimbursementPayslip();
  }

  downloadPayslip(): void {
    // Mock download - in real app would trigger file download
    console.log('Downloading payslip for', this.selectedMonth, this.selectedYear);
  }
}
