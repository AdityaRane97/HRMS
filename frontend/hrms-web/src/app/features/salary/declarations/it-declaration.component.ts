import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { SalaryService } from '../salary.service';
import { ITDeclaration, DeclarationCategory } from '../salary.model';

@Component({
  selector: 'app-it-declaration',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './it-declaration.component.html',
  styleUrls: ['./it-declaration.component.scss'],
})
export class ITDeclarationComponent implements OnInit {
  selectedFinancialYear = '2024-2025';
  financialYears = ['2024-2025', '2023-2024', '2022-2023'];

  itDeclaration: ITDeclaration | null = null;
  expandedCategories: Set<string> = new Set();
  editingCategoryId: string | null = null;
  editForm: FormGroup | null = null;

  loading = false;
  error: string | null = null;
  successMessage: string | null = null;

  constructor(
    private salaryService: SalaryService,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    this.loadITDeclaration();
  }

  loadITDeclaration(): void {
    this.loading = true;
    this.error = null;
    this.successMessage = null;
    this.salaryService.getITDeclaration(this.selectedFinancialYear).subscribe({
      next: (data) => {
        this.itDeclaration = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load IT declaration';
        this.loading = false;
      },
    });
  }

  onFinancialYearChange(): void {
    this.expandedCategories.clear();
    this.editingCategoryId = null;
    this.loadITDeclaration();
  }

  toggleCategory(categoryId: string): void {
    if (this.expandedCategories.has(categoryId)) {
      this.expandedCategories.delete(categoryId);
    } else {
      this.expandedCategories.add(categoryId);
    }
  }

  isCategoryExpanded(categoryId: string): boolean {
    return this.expandedCategories.has(categoryId);
  }

  editCategory(category: DeclarationCategory): void {
    this.editingCategoryId = category.id;
    this.editForm = this.formBuilder.group({
      declaredAmount: [category.declaredAmount],
    });
  }

  cancelEdit(): void {
    this.editingCategoryId = null;
    this.editForm = null;
  }

  saveCategory(): void {
    if (this.editForm && this.itDeclaration && this.editingCategoryId) {
      const category = this.itDeclaration.categories.find((c) => c.id === this.editingCategoryId);
      if (category && this.editForm.valid) {
        category.declaredAmount = this.editForm.get('declaredAmount')?.value;
        this.editingCategoryId = null;
        this.editForm = null;
        this.successMessage = 'Declaration updated successfully';
        setTimeout(() => (this.successMessage = null), 3000);
      }
    }
  }

  submitDeclaration(): void {
    if (this.itDeclaration) {
      this.loading = true;
      this.salaryService.updateITDeclaration(this.itDeclaration).subscribe({
        next: () => {
          this.successMessage = 'Declaration submitted successfully';
          this.loading = false;
          this.itDeclaration!.status = 'submitted';
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

  getTotalDeclared(): number {
    return this.itDeclaration?.categories.reduce((sum, cat) => sum + cat.declaredAmount, 0) || 0;
  }
}
