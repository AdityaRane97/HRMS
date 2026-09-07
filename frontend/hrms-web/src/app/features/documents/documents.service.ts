import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import {
  Document,
  DocumentCategory,
  DocumentFilter,
  DocumentListItem,
  DocumentUploadRequest,
  UpdateDocumentMetadataRequest,
} from './documents.model';
import { API_CONFIG } from '@core/config/api.config';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root',
})
export class DocumentsService {
  private apiUrl = `${API_CONFIG.baseUrl}/api/v1/documents`;
  private documentsSubject = new BehaviorSubject<DocumentListItem[]>([]);
  public documents$ = this.documentsSubject.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Add computed properties for UI display
   */
  private enrichDocumentWithComputedProps(doc: Document): DocumentListItem {
    return {
      ...doc,
      fileSizeDisplay: this.formatFileSize(doc.fileSize),
      uploadDateFormatted: this.formatDate(doc.uploadDate),
    };
  }

  /**
   * Format file size for display
   */
  private formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  /**
   * Format date for display
   */
  private formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  }

  /**
   * Get documents for the current authenticated employee
   * Returns only their own documents
   */
  getEmployeeDocuments(employeeId?: string): Observable<DocumentListItem[]> {
    return this.http
      .get<ApiResponse<Document[]>>(`${this.apiUrl}/my-documents`)
      .pipe(
        map(response => response.data || []),
        map(docs =>
          docs.map(doc => this.enrichDocumentWithComputedProps(doc))
        ),
        tap(docs => this.documentsSubject.next(docs))
      );
  }

  /**
   * Get all documents (admin/HR only)
   */
  getAllDocuments(): Observable<DocumentListItem[]> {
    return this.http
      .get<ApiResponse<Document[]>>(this.apiUrl)
      .pipe(
        map(response => response.data || []),
        map(docs =>
          docs.map(doc => this.enrichDocumentWithComputedProps(doc))
        )
      );
  }

  /**
   * Get documents for a specific employee (admin/HR only)
   */
  getEmployeeDocumentsByAdmin(employeeId: string): Observable<DocumentListItem[]> {
    return this.http
      .get<ApiResponse<Document[]>>(`${this.apiUrl}/employee/${employeeId}`)
      .pipe(
        map(response => response.data || []),
        map(docs =>
          docs.map(doc => this.enrichDocumentWithComputedProps(doc))
        )
      );
  }

  /**
   * Search and filter documents
   */
  searchDocuments(filter?: Partial<DocumentFilter>): Observable<DocumentListItem[]> {
    const params = new URLSearchParams();

    if (filter?.searchTerm) {
      params.append('searchTerm', filter.searchTerm);
    }
    if (filter?.category) {
      params.append('category', filter.category);
    }

    const queryString = params.toString();
    const url = queryString ? `${this.apiUrl}/search?${queryString}` : `${this.apiUrl}/search`;

    return this.http
      .get<ApiResponse<Document[]>>(url)
      .pipe(
        map(response => response.data || []),
        map(docs =>
          docs.map(doc => this.enrichDocumentWithComputedProps(doc))
        )
      );
  }

  /**
   * Get a specific document by ID
   */
  getDocument(documentId: string): Observable<Document> {
    return this.http
      .get<ApiResponse<Document>>(`${this.apiUrl}/${documentId}`)
      .pipe(map(response => response.data));
  }

  /**
   * Upload a new document
   * For admin/HR: can specify employeeId
   * For employees: employeeId is ignored and uses their own ID from JWT
   */
  uploadDocument(request: DocumentUploadRequest): Observable<Document> {
    const formData = new FormData();
    formData.append('employeeId', request.employeeId);
    formData.append('documentName', request.documentName);
    formData.append('category', request.category);
    if (request.description) {
      formData.append('description', request.description);
    }
    formData.append('file', request.file);

    return this.http
      .post<ApiResponse<Document>>(`${this.apiUrl}/upload`, formData)
      .pipe(map(response => response.data));
  }

  /**
   * Update document metadata
   */
  updateDocumentMetadata(
    documentId: string,
    request: UpdateDocumentMetadataRequest
  ): Observable<Document> {
    return this.http
      .put<ApiResponse<Document>>(
        `${this.apiUrl}/${documentId}/metadata`,
        request
      )
      .pipe(map(response => response.data));
  }

  /**
   * Replace a document file
   */
  replaceDocumentFile(
    documentId: string,
    file: File
  ): Observable<Document> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http
      .post<ApiResponse<Document>>(
        `${this.apiUrl}/${documentId}/replace-file`,
        formData
      )
      .pipe(map(response => response.data));
  }

  /**
   * Delete a document
   */
  deleteDocument(documentId: string): Observable<void> {
    return this.http
      .delete<void>(`${this.apiUrl}/${documentId}`);
  }

  /**
   * Download a document file
   */
  downloadDocument(documentId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${documentId}/download`, {
      responseType: 'blob',
    });
  }

  /**
   * Get a preview URL for a document
   */
  getPreviewUrl(documentId: string): string {
    return `${this.apiUrl}/${documentId}/download`;
  }

  /**
   * Get category display name
   */
  getCategoryDisplayName(category: DocumentCategory): string {
    const categoryMap: Record<DocumentCategory, string> = {
      [DocumentCategory.OfferLetter]: 'Offer Letter',
      [DocumentCategory.AppointmentLetter]: 'Appointment Letter',
      [DocumentCategory.Payslip]: 'Payslip',
      [DocumentCategory.TaxDocuments]: 'Tax Documents',
      [DocumentCategory.IdentityDocuments]: 'Identity Documents',
      [DocumentCategory.Certificates]: 'Certificates',
      [DocumentCategory.Policies]: 'Policies',
      [DocumentCategory.Other]: 'Other',
    };
    return categoryMap[category] || 'Document';
  }

  /**
   * Get all available document categories
   */
  getAvailableCategories(): DocumentCategory[] {
    return Object.values(DocumentCategory);
  }
}
