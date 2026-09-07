import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskAttachment } from '@features/tasks/tasks.model';

@Component({
  selector: 'app-task-attachments',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="bg-gray-50 rounded-lg p-4">
      <h3 class="font-semibold text-gray-900 mb-3">Attachments</h3>

      <div *ngIf="!attachments || attachments.length === 0" class="text-center py-6">
        <p class="text-gray-500">No attachments</p>
      </div>

      <div
        *ngIf="attachments && attachments.length > 0"
        class="space-y-2"
      >
        <div
          *ngFor="let attachment of attachments"
          class="flex items-center justify-between bg-white rounded p-3 border border-gray-200 hover:border-gray-300 transition"
        >
          <div class="flex items-center gap-3 flex-1">
            <span class="text-2xl">{{ getFileIcon(attachment.fileType) }}</span>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-gray-900 truncate">
                {{ attachment.fileName }}
              </p>
              <p class="text-xs text-gray-500">
                {{ formatFileSize(attachment.fileSize) }} •
                {{ formatDate(attachment.uploadDate) }}
              </p>
            </div>
          </div>
          <div class="flex gap-2">
            <button
              (click)="onDownload.emit(attachment)"
              class="px-3 py-1 text-sm font-medium text-blue-600 hover:text-blue-800"
              title="Download"
            >
              📥
            </button>
            <button
              *ngIf="deletable"
              (click)="onDelete.emit(attachment)"
              class="px-3 py-1 text-sm font-medium text-red-600 hover:text-red-800"
              title="Delete"
            >
              🗑️
            </button>
          </div>
        </div>
      </div>

      <div *ngIf="uploadable" class="mt-4">
        <input
          #fileInput
          type="file"
          hidden
          (change)="onFileSelected($event)"
          multiple
        />
        <button
          (click)="fileInput.click()"
          class="w-full px-4 py-2 border-2 border-dashed border-gray-300 rounded-lg text-sm font-medium text-gray-700 hover:border-blue-500 hover:text-blue-600 transition"
        >
          + Add Attachment
        </button>
      </div>
    </div>
  `,
})
export class TaskAttachmentsComponent {
  @Input() attachments: TaskAttachment[] = [];
  @Input() uploadable = false;
  @Input() deletable = false;

  @Output() onDownload = new EventEmitter<TaskAttachment>();
  @Output() onDelete = new EventEmitter<TaskAttachment>();
  @Output() onUpload = new EventEmitter<File[]>();

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      const files = Array.from(input.files);
      this.onUpload.emit(files);
      // Reset input
      input.value = '';
    }
  }

  getFileIcon(fileType: string): string {
    if (fileType.includes('pdf')) return '📄';
    if (fileType.includes('image')) return '🖼️';
    if (
      fileType.includes('sheet') ||
      fileType.includes('excel') ||
      fileType.includes('csv')
    )
      return '📊';
    if (fileType.includes('word') || fileType.includes('document'))
      return '📝';
    if (fileType.includes('zip') || fileType.includes('archive'))
      return '📦';
    return '📎';
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + ' ' + sizes[i];
  }

  formatDate(date: Date): string {
    const d = new Date(date);
    return d.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });
  }
}
