// Payslip Models
export interface SalaryComponent {
  name: string;
  amount: number;
}

export interface Payslip {
  id: string;
  employeeId: string;
  employeeName: string;
  employeeNumber: string;
  month: string;
  year: number;
  joiningDate: string;
  bankName: string;
  bankAccountNumber: string;
  earnings: SalaryComponent[];
  deductions: SalaryComponent[];
  totalEarnings: number;
  totalDeductions: number;
  netPay: number;
  netPayInWords: string;
}

// CTC Payslip Models
export interface CTCComponent {
  name: string;
  annualAmount: number;
}

export interface CTCPayslip {
  id: string;
  employeeId: string;
  employeeName: string;
  employeeNumber: string;
  financialYear: string;
  components: CTCComponent[];
  totalCTC: number;
  effectiveDate: string;
}

// Reimbursement Models
export interface ReimbursementPayslip {
  id: string;
  employeeId: string;
  employeeName: string;
  month: string;
  year: number;
  components: SalaryComponent[];
  totalReimbursement: number;
}

// YTD Report Models
export interface YTDReportRow {
  label: string;
  values: number[];
  isTotal?: boolean;
  isSubtotal?: boolean;
  expandable?: boolean;
  children?: YTDReportRow[];
}

export interface YTDReport {
  employeeId: string;
  financialYear: string;
  months: string[];
  incomeRows: YTDReportRow[];
  deductionRows: YTDReportRow[];
  daysRows: YTDReportRow[];
  summaryRows: YTDReportRow[];
}

// Declaration Models
export interface DeclarationCategory {
  id: string;
  name: string;
  section: string; // e.g., "80C", "Medical", etc.
  maxLimit: number;
  declaredAmount: number;
  expandable: boolean;
  subCategories?: DeclarationSubCategory[];
}

export interface DeclarationSubCategory {
  id: string;
  name: string;
  declaredAmount: number;
  maxLimit: number;
}

export interface ITDeclaration {
  id: string;
  employeeId: string;
  financialYear: string;
  categories: DeclarationCategory[];
  annualTax: number;
  taxPaidTillDate: number;
  balancePayable: number;
  status: 'draft' | 'submitted' | 'verified';
}

// FBP Declaration Models
export interface FBPComponent {
  id: string;
  name: string;
  maximumLimit: number;
  declaredAmount: number;
  consideredInPayroll: number;
}

export interface FBPDeclaration {
  id: string;
  employeeId: string;
  financialYear: string;
  components: FBPComponent[];
  totalDeclared: number;
  totalConsidered: number;
  specialAllowance: number;
  remainingAmount: number;
  status: 'draft' | 'submitted' | 'verified';
}

// Salary Summary Models
export interface SalarySummary {
  currentMonth: string;
  currentYear: number;
  netPayCurrentMonth: number;
  netPayYTD: number;
  pendingPayslips: number;
  ctcAmount: number;
}

// Quick Access Models
export interface QuickAccessItem {
  title: string;
  route: string;
  icon: string;
  description?: string;
}
