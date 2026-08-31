import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import {
  Payslip,
  CTCPayslip,
  ReimbursementPayslip,
  YTDReport,
  ITDeclaration,
  FBPDeclaration,
  SalarySummary,
} from './salary.model';

@Injectable({
  providedIn: 'root',
})
export class SalaryService {
  private mockPayslips: Payslip[] = [
    {
      id: '1',
      employeeId: 'EMP001',
      employeeName: 'John Doe',
      employeeNumber: 'EMP001',
      month: 'January',
      year: 2024,
      joiningDate: '2022-03-15',
      bankName: 'State Bank of India',
      bankAccountNumber: '****1234',
      earnings: [
        { name: 'Basic', amount: 50000 },
        { name: 'HRA', amount: 15000 },
        { name: 'Flexible Allowance', amount: 10000 },
        { name: 'Telephone Allowance', amount: 1500 },
        { name: 'LTA', amount: 2500 },
        { name: 'Vehicle Maintenance Allowance', amount: 5000 },
      ],
      deductions: [
        { name: 'PF', amount: 5000 },
        { name: 'Income Tax', amount: 8000 },
        { name: 'Professional Tax', amount: 200 },
      ],
      totalEarnings: 84000,
      totalDeductions: 13200,
      netPay: 70800,
      netPayInWords: 'Seventy Thousand Eight Hundred Only',
    },
  ];

  private mockCTCPayslip: CTCPayslip = {
    id: '1',
    employeeId: 'EMP001',
    employeeName: 'John Doe',
    employeeNumber: 'EMP001',
    financialYear: '2024-2025',
    components: [
      { name: 'Annual Basic', annualAmount: 600000 },
      { name: 'Annual HRA', annualAmount: 180000 },
      { name: 'Annual Flexible Allowance', annualAmount: 120000 },
      { name: 'Annual Bonus', annualAmount: 50000 },
      { name: 'Annual Gratuity', annualAmount: 25000 },
      { name: 'Annual Company PF', annualAmount: 60000 },
    ],
    totalCTC: 1035000,
    effectiveDate: '2024-01-01',
  };

  private mockYTDReport: YTDReport = {
    employeeId: 'EMP001',
    financialYear: '2024-2025',
    months: ['Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec', 'Jan'],
    incomeRows: [
      {
        label: 'Income',
        values: [],
        expandable: true,
        children: [
          {
            label: 'Basic',
            values: [50000, 50000, 50000, 50000, 50000, 50000, 50000, 50000],
          },
          {
            label: 'HRA',
            values: [15000, 15000, 15000, 15000, 15000, 15000, 15000, 15000],
          },
          {
            label: 'Flexible Allowance',
            values: [10000, 10000, 10000, 10000, 10000, 10000, 10000, 10000],
          },
          {
            label: 'Telephone Allowance',
            values: [1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500],
          },
          {
            label: 'Gross Income',
            values: [76500, 76500, 76500, 76500, 76500, 76500, 76500, 76500],
            isSubtotal: true,
          },
        ],
      },
    ],
    deductionRows: [
      {
        label: 'Deductions',
        values: [],
        expandable: true,
        children: [
          {
            label: 'PF',
            values: [5000, 5000, 5000, 5000, 5000, 5000, 5000, 5000],
          },
          {
            label: 'Income Tax',
            values: [8000, 8000, 8000, 8000, 8000, 8000, 8000, 8000],
          },
          {
            label: 'Professional Tax',
            values: [200, 200, 200, 200, 200, 200, 200, 200],
          },
          {
            label: 'Total Deductions',
            values: [13200, 13200, 13200, 13200, 13200, 13200, 13200, 13200],
            isSubtotal: true,
          },
        ],
      },
    ],
    daysRows: [
      {
        label: 'Effective Working Days',
        values: [22, 22, 22, 21, 22, 21, 22, 20],
      },
      {
        label: 'Days in Month',
        values: [30, 31, 31, 30, 31, 30, 31, 31],
      },
    ],
    summaryRows: [
      {
        label: 'Net Pay',
        values: [63300, 63300, 63300, 63300, 63300, 63300, 63300, 63300],
        isTotal: true,
      },
    ],
  };

  private mockITDeclaration: ITDeclaration = {
    id: '1',
    employeeId: 'EMP001',
    financialYear: '2024-2025',
    categories: [
      {
        id: '1',
        name: 'Section 10(13) House Rent Allowance',
        section: '10(13)',
        maxLimit: 500000,
        declaredAmount: 150000,
        expandable: false,
      },
      {
        id: '2',
        name: '80C Deductions',
        section: '80C',
        maxLimit: 150000,
        declaredAmount: 150000,
        expandable: true,
        subCategories: [
          { id: '2a', name: 'Life Insurance Premium', declaredAmount: 50000, maxLimit: 100000 },
          { id: '2b', name: 'PPF Contributions', declaredAmount: 50000, maxLimit: 150000 },
          { id: '2c', name: 'Home Loan Principal', declaredAmount: 50000, maxLimit: 150000 },
        ],
      },
      {
        id: '3',
        name: 'Medical Deductions',
        section: '80D',
        maxLimit: 100000,
        declaredAmount: 50000,
        expandable: true,
        subCategories: [
          { id: '3a', name: 'Health Insurance Premium', declaredAmount: 50000, maxLimit: 100000 },
        ],
      },
      {
        id: '4',
        name: 'Housing Deductions',
        section: 'Others',
        maxLimit: 200000,
        declaredAmount: 75000,
        expandable: false,
      },
    ],
    annualTax: 50000,
    taxPaidTillDate: 35000,
    balancePayable: 15000,
    status: 'submitted',
  };

  private mockFBPDeclaration: FBPDeclaration = {
    id: '1',
    employeeId: 'EMP001',
    financialYear: '2024-2025',
    components: [
      { id: '1', name: 'Children Hostel Reimbursement', maximumLimit: 50000, declaredAmount: 30000, consideredInPayroll: 30000 },
      { id: '2', name: 'Telephone Expenses', maximumLimit: 20000, declaredAmount: 15000, consideredInPayroll: 15000 },
      { id: '3', name: 'Leave Travel Assistance', maximumLimit: 100000, declaredAmount: 50000, consideredInPayroll: 50000 },
      { id: '4', name: 'Vehicle Maintenance', maximumLimit: 40000, declaredAmount: 30000, consideredInPayroll: 30000 },
      { id: '5', name: 'Food Coupon Allowance', maximumLimit: 60000, declaredAmount: 60000, consideredInPayroll: 60000 },
      { id: '6', name: 'Books/Periodical Reimbursement', maximumLimit: 15000, declaredAmount: 10000, consideredInPayroll: 10000 },
      { id: '7', name: 'Children Education Reimbursement', maximumLimit: 50000, declaredAmount: 35000, consideredInPayroll: 35000 },
      { id: '8', name: 'FBP VP', maximumLimit: 100000, declaredAmount: 75000, consideredInPayroll: 75000 },
    ],
    totalDeclared: 305000,
    totalConsidered: 305000,
    specialAllowance: 0,
    remainingAmount: 120000,
    status: 'submitted',
  };

  constructor() {}

  getPayslip(month: string, year: number): Observable<Payslip> {
    // In a real scenario, this would call the backend
    return of(this.mockPayslips[0]);
  }

  getPayslips(): Observable<Payslip[]> {
    return of(this.mockPayslips);
  }

  getCTCPayslip(year: string): Observable<CTCPayslip> {
    return of(this.mockCTCPayslip);
  }

  getReimbursementPayslip(month: string, year: number): Observable<ReimbursementPayslip> {
    return of({
      id: '1',
      employeeId: 'EMP001',
      employeeName: 'John Doe',
      month,
      year,
      components: [
        { name: 'Food Coupon Allowance', amount: 5000 },
        { name: 'Leave Travel Assistance', amount: 5000 },
        { name: 'Vehicle Maintenance', amount: 3000 },
      ],
      totalReimbursement: 13000,
    });
  }

  getYTDReport(financialYear: string): Observable<YTDReport> {
    return of(this.mockYTDReport);
  }

  getITDeclaration(financialYear: string): Observable<ITDeclaration> {
    return of(this.mockITDeclaration);
  }

  getFBPDeclaration(financialYear: string): Observable<FBPDeclaration> {
    return of(this.mockFBPDeclaration);
  }

  getSalarySummary(): Observable<SalarySummary> {
    return of({
      currentMonth: 'January',
      currentYear: 2024,
      netPayCurrentMonth: 70800,
      netPayYTD: 566400, // 8 months
      pendingPayslips: 2,
      ctcAmount: 1035000,
    });
  }

  updateITDeclaration(declaration: ITDeclaration): Observable<ITDeclaration> {
    // Mock save - in real app would be POST/PUT to backend
    this.mockITDeclaration = declaration;
    return of(declaration);
  }

  updateFBPDeclaration(declaration: FBPDeclaration): Observable<FBPDeclaration> {
    // Mock save - in real app would be POST/PUT to backend
    this.mockFBPDeclaration = declaration;
    return of(declaration);
  }
}
