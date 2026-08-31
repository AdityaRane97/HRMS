import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SalaryService } from '../salary.service';
import { YTDReport, YTDReportRow } from '../salary.model';

@Component({
  selector: 'app-ytd-reports',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ytd-reports.component.html',
  styleUrls: ['./ytd-reports.component.scss'],
})
export class YTDReportsComponent implements OnInit {
  activeTab: 'ytd' | 'fy' = 'ytd';
  selectedFinancialYear = '2024-2025';
  financialYears = ['2024-2025', '2023-2024', '2022-2023'];

  ytdReport: YTDReport | null = null;
  expandedRows: Set<string> = new Set();

  loading = false;
  error: string | null = null;

  constructor(private salaryService: SalaryService) {}

  ngOnInit(): void {
    this.loadYTDReport();
  }

  loadYTDReport(): void {
    this.loading = true;
    this.error = null;
    this.salaryService.getYTDReport(this.selectedFinancialYear).subscribe({
      next: (data) => {
        this.ytdReport = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load YTD report';
        this.loading = false;
      },
    });
  }

  onFinancialYearChange(): void {
    this.expandedRows.clear();
    this.loadYTDReport();
  }

  toggleRow(label: string): void {
    if (this.expandedRows.has(label)) {
      this.expandedRows.delete(label);
    } else {
      this.expandedRows.add(label);
    }
  }

  isRowExpanded(label: string): boolean {
    return this.expandedRows.has(label);
  }

  selectTab(tab: 'ytd' | 'fy'): void {
    this.activeTab = tab;
  }
}
