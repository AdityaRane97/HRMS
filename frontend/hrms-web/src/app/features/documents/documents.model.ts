export enum DocumentCategory {
  OfferLetter = 'OFFER_LETTER',
  AppointmentLetter = 'APPOINTMENT_LETTER',
  Payslip = 'PAYSLIP',
  TaxDocuments = 'TAX_DOCUMENTS',
  IdentityDocuments = 'IDENTITY_DOCUMENTS',
  Certificates = 'CERTIFICATES',
  Policies = 'POLICIES',
  Other = 'OTHER',
}

export interface Document {
  id: string;
  employeeId: string;
  employeeName: string;
  documentName: string;
  category: DocumentCategory;
  description?: string;
  fileName: string;
  fileType: string;
  fileSize: number; // in bytes
  uploadDate: string; // ISO date string
  isActive: boolean;
  documentUrl?: string;
  uploadedBy?: string; // Email of who uploaded it (for admin uploads)
}

export interface DocumentListItem extends Document {
  // Computed properties for UI
  fileSizeDisplay?: string; // "1.2 MB", "500 KB", etc.
  uploadDateFormatted?: string; // Readable date format
}

export interface DocumentUploadRequest {
  employeeId: string;
  documentName: string;
  category: DocumentCategory;
  description?: string;
  file: File;
}

export interface DocumentFilter {
  searchTerm?: string;
  category?: DocumentCategory;
  startDate?: string;
  endDate?: string;
}

export interface DocumentResponse {
  id: string;
  employeeId: string;
  employeeName: string;
  documentName: string;
  category: string;
  description?: string;
  fileName: string;
  fileType: string;
  fileSize: number;
  uploadDate: string;
  isActive: boolean;
}

export interface UpdateDocumentMetadataRequest {
  documentName?: string;
  category?: DocumentCategory;
  description?: string;
}
