import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SalaryService } from '../salary.service';
import { FBPDeclaration } from '../salary.model';

@Component({
  selector: 'app-fbp-declaration',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './fbp-declaration.component.html',
  styleUrls: ['./fbp-declaration.component.scss'],
})
export class FBPDeclarationComponent implements OnInit {
  selectedFinancialYear = '2024-2025';
  financialYears = ['2024-2025', '2023-2024', '2022-2023'];

  fbpDeclaration: FBPDeclaration | null = null;
  editingComponentId: string | null = null;
  editAmount: number | null = null;

  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  constructor(private salaryService: SalaryService) {}

  ngOnInit(): void {
    this.loadFBPDeclaration();
  }

  loadFBPDeclaration(): void {
    this.loading = true;
    this.error = null;
    this.successMessage = null;
    this.salaryService.getFBPDeclaration(this.selectedFinancialYear).subscribe({
      next: (data) => {
        this.fbpDeclaration = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load FBP declaration';
        this.loading = false;
      },
    });
  }

  onFinancialYearChange(): void {
    this.editingComponentId = null;
    this.loadFBPDeclaration();
  }

  editComponent(componentId: string, amount: number): void {
    this.editingComponentId = componentId;
    this.editAmount = amount;
  }

  cancelEdit(): void {
    this.editingComponentId = null;
    this.editAmount = null;
  }

  saveComponent(): void {
    if (this.editingComponentId && this.fbpDeclaration && this.editAmount !== null) {
      const component = this.fbpDeclaration.components.find((c) => c.id === this.editingComponentId);
      if (component) {
        component.declaredAmount = this.editAmount;
        // Update Considered in Payroll (same as declared for mock)
        component.consideredInPayroll = this.editAmount;
        // Recalculate totals
        this.fbpDeclaration.totalDeclared = this.fbpDeclaration.components.reduce(
          (sum, c) => sum + c.declaredAmount,
          0
        );
        this.fbpDeclaration.totalConsidered = this.fbpDeclaration.components.reduce(
          (sum, c) => sum + c.consideredInPayroll,
          0
        );
        // Calculate remaining
        const totalLimit = this.fbpDeclaration.components.reduce((sum, c) => sum + c.maximumLimit, 0);
        this.fbpDeclaration.remainingAmount = totalLimit - this.fbpDeclaration.totalDeclared;

        this.editingComponentId = null;
        this.editAmount = null;
        this.successMessage = 'Declaration updated successfully';
        setTimeout(() => (this.successMessage = null), 3000);
      }
    }
  }

  submitDeclaration(): void {
    if (this.fbpDeclaration) {
      this.loading = true;
      this.salaryService.updateFBPDeclaration(this.fbpDeclaration).subscribe({
        next: () => {
          this.successMessage = 'Declaration submitted successfully';
          this.loading = false;
          this.fbpDeclaration!.status = 'submitted';
          setTimeout(() => (this.successMessage = null), 3000);
        },
        error: () => {
          this.error = 'Failed to submit declaration';
          this.loading = false;
        },
      });
    }
  }

  getStatusColor(status: string): string {
    if (status === 'submitted') return 'bg-green-100 text-green-700';
    if (status === 'verified') return 'bg-blue-100 text-blue-700';
    return 'bg-yellow-100 text-yellow-700';
  }
}
