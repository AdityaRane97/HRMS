import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { DocumentsService } from '../documents.service';
import {
  DocumentListItem,
  DocumentCategory,
  DocumentFilter,
} from '../documents.model';
import { AuthService } from '@core/auth/auth.service';

@Component({
  selector: 'app-employee-documents',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-documents.component.html',
  styleUrls: ['./employee-documents.component.scss'],
})
export class EmployeeDocumentsComponent implements OnInit {
  documents: DocumentListItem[] = [];
  filteredDocuments: DocumentListItem[] = [];

  // UI State
  loading = false;
  error: string | null = null;
  selectedDocument: DocumentListItem | null = null;
  showPreview = false;
  expandedSortMenu = false;
  expandedCategoryMenu = false;

  // Filter and Search
  searchTerm = '';
  selectedCategory: DocumentCategory | '' = '';
  sortBy: 'name' | 'date' | 'size' = 'date';
  sortAscending = false;

  // Categories for filter dropdown
  DocumentCategory = DocumentCategory;
  availableCategories: DocumentCategory[] = [];

  // Current user
  currentUserId: string = '';
  currentUserName: string = '';

  // Empty state
  get isEmpty(): boolean {
    return !this.loading && this.documents.length === 0;
  }

  // No results state
  get hasNoResults(): boolean {
    return !this.loading && this.documents.length > 0 && this.filteredDocuments.length === 0;
  }

  constructor(
    private documentsService: DocumentsService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.currentUserId = user.id;
      this.currentUserName = user.firstName || 'User';
      this.loadDocuments();
      this.availableCategories = this.documentsService.getAvailableCategories();
    }
  }

  /**
   * Load documents for current employee
   */
  loadDocuments(): void {
    this.loading = true;
    this.error = null;

    this.documentsService.getEmployeeDocuments(this.currentUserId).subscribe({
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
   * Apply search and category filters
   */
  applyFilters(): void {
    const filter: DocumentFilter = {
      searchTerm: this.searchTerm,
      category: this.selectedCategory ? this.selectedCategory : undefined,
    };

    this.documentsService.searchDocuments(filter).subscribe({
      next: (docs) => {
        this.filteredDocuments = docs;
        this.sortDocuments();
      },
      error: (err) => {
        console.error('Error filtering documents:', err);
      },
    });
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
  onSortChange(sortBy: 'name' | 'date' | 'size'): void {
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
   * Select document for preview/actions
   */
  selectDocument(doc: DocumentListItem): void {
    this.selectedDocument = doc;
  }

  /**
   * Clear document selection
   */
  clearSelection(): void {
    this.selectedDocument = null;
    this.showPreview = false;
  }

  /**
   * Preview document
   */
  previewDocument(doc: DocumentListItem): void {
    this.selectDocument(doc);
    this.showPreview = true;
  }

  /**
   * Download document
   */
  downloadDocument(doc: DocumentListItem, event: Event): void {
    event.stopPropagation();

    // Show loading state
    const originalName = doc.documentName;
    doc.documentName = 'Downloading...';

    this.documentsService.downloadDocument(doc.id).subscribe({
      next: (blob) => {
        // Restore original name
        doc.documentName = originalName;

        // Create blob URL and trigger download
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
        // Restore original name
        doc.documentName = originalName;
        this.error = 'Failed to download document. Please try again.';
        console.error('Error downloading document:', err);
      },
    });
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
   * Retry loading documents
   */
  retryLoad(): void {
    this.loadDocuments();
  }
}
