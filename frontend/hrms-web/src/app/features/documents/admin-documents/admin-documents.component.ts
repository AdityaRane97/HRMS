import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DocumentsService } from '../documents.service';
import {
  DocumentListItem,
  DocumentCategory,
  DocumentFilter,
  DocumentUploadRequest,
  UpdateDocumentMetadataRequest,
  Document as DocumentModel,
} from '../documents.model';
import { AuthService } from '@core/auth/auth.service';

interface EmployeeOption {
  id: string;
  name: string;
}

@Component({
  selector: 'app-admin-documents',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-documents.component.html',
  styleUrls: ['./admin-documents.component.scss'],
})
export class AdminDocumentsComponent implements OnInit {
  documents: DocumentListItem[] = [];
  filteredDocuments: DocumentListItem[] = [];

  // UI State
  loading = false;
  error: string | null = null;
  successMessage: string | null = null;
  selectedDocument: DocumentListItem | null = null;
  showUploadForm = false;
  showEditForm = false;
  showReplaceForm = false;
  showDeleteConfirm = false;
  expandedSortMenu = false;
  expandedCategoryMenu = false;

  // Filter and Search
  searchTerm = '';
  selectedEmployeeFilter: string = '';
  selectedCategory: DocumentCategory | '' = '';
  sortBy: 'name' | 'date' | 'size' | 'employee' = 'date';
  sortAscending = false;

  // Upload Form State
  uploadForm = {
    employeeId: '',
    documentName: '',
    category: '' as DocumentCategory | '',
    description: '',
    file: null as File | null,
  };

  uploadProgress = 0;
  uploading = false;

  // Edit Form State
  editForm = {
    documentName: '',
    category: '' as DocumentCategory | '',
    description: '',
  };

  editing = false;

  // Replace File State
  replaceFile: File | null = null;
  replacing = false;

  // Mock employee data - in real app, would come from backend
  employees: EmployeeOption[] = [
    { id: 'EMP001', name: 'John Doe' },
    { id: 'EMP002', name: 'Jane Smith' },
    { id: 'EMP003', name: 'Bob Johnson' },
    { id: 'EMP004', name: 'Alice Brown' },
  ];

  // Categories for filter dropdown
  DocumentCategory = DocumentCategory;
  availableCategories: DocumentCategory[] = [];

  // Empty state
  get isEmpty(): boolean {
    return !this.loading && this.documents.length === 0;
  }

  // No results state
  get hasNoResults(): boolean {
    return (
      !this.loading &&
      this.documents.length > 0 &&
      this.filteredDocuments.length === 0
    );
  }

  // Form validation
  get isUploadFormValid(): boolean {
    return (
      !!this.uploadForm.employeeId &&
      !!this.uploadForm.documentName &&
      this.uploadForm.category !== '' &&
      !!this.uploadForm.file
    );
  }

  get isEditFormValid(): boolean {
    return !!this.editForm.documentName && this.editForm.category !== '';
  }

  constructor(
    private documentsService: DocumentsService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadDocuments();
    this.availableCategories = this.documentsService.getAvailableCategories();
  }

  /**
   * Load all documents (admin view)
   */
  loadDocuments(): void {
    this.loading = true;
    this.error = null;

    this.documentsService.getAllDocuments().subscribe({
      next: (docs) => {
        this.documents = docs;
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load documents. Please try again.';
        this.loading = false;
        console.error('Error loading documents:', err);
      },
    });
  }

  /**
   * Handle search input
   */
  onSearchChange(): void {
    this.applyFilters();
  }

  /**
   * Handle category filter change
   */
  onCategoryChange(): void {
    this.expandedCategoryMenu = false;
    this.applyFilters();
  }

  /**
   * Handle employee filter change
   */
  onEmployeeFilterChange(): void {
    this.applyFilters();
  }

  /**
   * Apply search and category filters
   */
  applyFilters(): void {
    const filter: DocumentFilter = {
      searchTerm: this.searchTerm,
      category: this.selectedCategory ? this.selectedCategory : undefined,
    };

    let filtered = this.documents;

    // Apply filter for employee if selected
    if (this.selectedEmployeeFilter) {
      filtered = filtered.filter(
        (d) => d.employeeId === this.selectedEmployeeFilter
      );
    }

    // Apply text and category filters
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(
        (d) =>
          d.documentName.toLowerCase().includes(term) ||
          d.description?.toLowerCase().includes(term) ||
          d.employeeName.toLowerCase().includes(term)
      );
    }

    if (this.selectedCategory) {
      filtered = filtered.filter((d) => d.category === this.selectedCategory);
    }

    this.filteredDocuments = filtered;
    this.sortDocuments();
  }

  /**
   * Sort documents
   */
  sortDocuments(): void {
    this.filteredDocuments.sort((a, b) => {
      let aValue: any;
      let bValue: any;

      switch (this.sortBy) {
        case 'name':
          aValue = a.documentName.toLowerCase();
          bValue = b.documentName.toLowerCase();
          break;
        case 'employee':
          aValue = a.employeeName.toLowerCase();
          bValue = b.employeeName.toLowerCase();
          break;
        case 'date':
          aValue = new Date(a.uploadDate).getTime();
          bValue = new Date(b.uploadDate).getTime();
          break;
        case 'size':
          aValue = a.fileSize;
          bValue = b.fileSize;
          break;
      }

      return this.sortAscending ? aValue - bValue : bValue - aValue;
    });
  }

  /**
   * Handle sort option change
   */
  onSortChange(sortBy: 'name' | 'date' | 'size' | 'employee'): void {
    if (this.sortBy === sortBy) {
      this.sortAscending = !this.sortAscending;
    } else {
      this.sortBy = sortBy;
      this.sortAscending = false;
    }
    this.expandedSortMenu = false;
    this.sortDocuments();
  }

  /**
   * Clear all filters and search
   */
  clearFilters(): void {
    this.searchTerm = '';
    this.selectedCategory = '';
    this.selectedEmployeeFilter = '';
    this.sortBy = 'date';
    this.sortAscending = false;
    this.applyFilters();
  }

  /**
   * Get display name for category
   */
  getCategoryDisplay(category: DocumentCategory): string {
    return this.documentsService.getCategoryDisplayName(category);
  }

  /**
   * Select document for actions
   */
  selectDocument(doc: DocumentListItem): void {
    this.selectedDocument = doc;
    this.clearMessages();
  }

  /**
   * Clear document selection and forms
   */
  clearSelection(): void {
    this.selectedDocument = null;
    this.showEditForm = false;
    this.showReplaceForm = false;
    this.showDeleteConfirm = false;
  }

  /**
   * Upload new document
   */
  submitUpload(): void {
    if (!this.isUploadFormValid) return;

    this.uploading = true;
    this.error = null;
    this.successMessage = null;

    const employeeName =
      this.employees.find((e) => e.id === this.uploadForm.employeeId)?.name ||
      'Unknown Employee';

    const request: DocumentUploadRequest = {
      employeeId: this.uploadForm.employeeId,
      documentName: this.uploadForm.documentName,
      category: this.uploadForm.category as DocumentCategory,
      description: this.uploadForm.description,
      file: this.uploadForm.file!,
    };

    this.documentsService.uploadDocument(request).subscribe({
      next: (doc) => {
        this.successMessage = `Document "${this.uploadForm.documentName}" uploaded successfully!`;
        this.resetUploadForm();
        this.loadDocuments();
        this.uploading = false;

        // Close upload form after 2 seconds
        setTimeout(() => {
          this.showUploadForm = false;
        }, 2000);
      },
      error: (err) => {
        this.error = 'Failed to upload document. Please try again.';
        this.uploading = false;
        console.error('Error uploading document:', err);
      },
    });
  }

  /**
   * Reset upload form
   */
  resetUploadForm(): void {
    this.uploadForm = {
      employeeId: '',
      documentName: '',
      category: '',
      description: '',
      file: null,
    };
    this.uploadProgress = 0;
  }

  /**
   * Handle file selection for upload
   */
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.uploadForm.file = input.files[0];
    }
  }

  /**
   * Handle file selection for replace
   */
  onReplaceFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.replaceFile = input.files[0];
    }
  }

  /**
   * Open edit form for selected document
   */
  openEditForm(): void {
    if (!this.selectedDocument) return;

    this.editForm = {
      documentName: this.selectedDocument.documentName,
      category: this.selectedDocument.category,
      description: this.selectedDocument.description || '',
    };
    this.showEditForm = true;
  }

  /**
   * Submit edit form
   */
  submitEdit(): void {
    if (!this.selectedDocument || !this.isEditFormValid) return;

    this.editing = true;
    this.error = null;

    const request: UpdateDocumentMetadataRequest = {
      documentName: this.editForm.documentName,
      category: this.editForm.category as DocumentCategory,
      description: this.editForm.description,
    };

    this.documentsService
      .updateDocumentMetadata(this.selectedDocument.id, request)
      .subscribe({
        next: (doc) => {
          this.successMessage = 'Document metadata updated successfully!';
          this.showEditForm = false;
          this.editing = false;
          this.clearSelection();
          this.loadDocuments();

          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err) => {
          this.error = 'Failed to update document metadata. Please try again.';
          this.editing = false;
          console.error('Error updating document:', err);
        },
      });
  }

  /**
   * Open replace file form
   */
  openReplaceForm(): void {
    this.showReplaceForm = true;
    this.replaceFile = null;
  }

  /**
   * Submit replace file
   */
  submitReplace(): void {
    if (!this.selectedDocument || !this.replaceFile) return;

    this.replacing = true;
    this.error = null;

    this.documentsService
      .replaceDocumentFile(this.selectedDocument.id, this.replaceFile!)
      .subscribe({
        next: (doc: DocumentModel) => {
          this.successMessage = 'Document file replaced successfully!';
          this.showReplaceForm = false;
          this.replacing = false;
          this.replaceFile = null;
          this.clearSelection();
          this.loadDocuments();

          setTimeout(() => {
            this.successMessage = null;
          }, 3000);
        },
        error: (err: any) => {
          this.error = 'Failed to replace document file. Please try again.';
          this.replacing = false;
          console.error('Error replacing document:', err);
        },
      });
  }

  /**
   * Open delete confirmation
   */
  openDeleteConfirm(): void {
    this.showDeleteConfirm = true;
  }

  /**
   * Confirm delete document
   */
  confirmDelete(): void {
    if (!this.selectedDocument) return;

    const documentName = this.selectedDocument.documentName;

    this.documentsService.deleteDocument(this.selectedDocument.id).subscribe({
      next: () => {
        this.successMessage = `Document "${documentName}" deleted successfully!`;
        this.showDeleteConfirm = false;
        this.clearSelection();
        this.loadDocuments();

        setTimeout(() => {
          this.successMessage = null;
        }, 3000);
      },
      error: (err) => {
        this.error = 'Failed to delete document. Please try again.';
        console.error('Error deleting document:', err);
      },
    });
  }

  /**
   * Download document
   */
  downloadDocument(doc: DocumentListItem): void {
    this.documentsService.downloadDocument(doc.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = doc.fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        this.error = 'Failed to download document. Please try again.';
        console.error('Error downloading document:', err);
      },
    });
  }

  /**
   * Clear success/error messages
   */
  clearMessages(): void {
    this.error = null;
    this.successMessage = null;
  }

  /**
   * Toggle sort menu
   */
  toggleSortMenu(): void {
    this.expandedSortMenu = !this.expandedSortMenu;
    this.expandedCategoryMenu = false;
  }

  /**
   * Toggle category menu
   */
  toggleCategoryMenu(): void {
    this.expandedCategoryMenu = !this.expandedCategoryMenu;
    this.expandedSortMenu = false;
  }

  /**
   * Close menus when clicking outside
   */
  closeMenus(): void {
    this.expandedSortMenu = false;
    this.expandedCategoryMenu = false;
  }

  /**
   * Get file icon based on file type
   */
  getFileIcon(fileType: string): string {
    if (fileType.includes('pdf')) return '📄';
    if (fileType.includes('image')) return '🖼️';
    if (fileType.includes('word') || fileType.includes('document'))
      return '📝';
    if (fileType.includes('sheet') || fileType.includes('excel')) return '📊';
    if (fileType.includes('presentation')) return '📽️';
    return '📎';
  }

  /**
   * Get category badge color
   */
  getCategoryBadgeColor(category: DocumentCategory): string {
    const colors: Record<DocumentCategory, string> = {
      [DocumentCategory.OfferLetter]: 'bg-blue-100 text-blue-800',
      [DocumentCategory.AppointmentLetter]: 'bg-green-100 text-green-800',
      [DocumentCategory.Payslip]: 'bg-purple-100 text-purple-800',
      [DocumentCategory.TaxDocuments]: 'bg-yellow-100 text-yellow-800',
      [DocumentCategory.IdentityDocuments]: 'bg-red-100 text-red-800',
      [DocumentCategory.Certificates]: 'bg-indigo-100 text-indigo-800',
      [DocumentCategory.Policies]: 'bg-gray-100 text-gray-800',
      [DocumentCategory.Other]: 'bg-cyan-100 text-cyan-800',
    };
    return colors[category] || 'bg-gray-100 text-gray-800';
  }

  /**
   * Get employee name by id
   */
  getEmployeeName(employeeId: string): string {
    return (
      this.employees.find((e) => e.id === employeeId)?.name || 'Unknown'
    );
  }

  /**
   * Retry loading documents
   */
  retryLoad(): void {
    this.loadDocuments();
  }
}
