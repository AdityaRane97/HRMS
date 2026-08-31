import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Reusable component for displaying profile information fields with label and value.
 * Supports optional masking and visibility toggle for sensitive data.
 * 
 * Usage:
 * <app-profile-info-field
 *   label="Email"
 *   [value]="employee.email"
 *   [maskable]="true"
 *   maskChar="•">
 * </app-profile-info-field>
 */
@Component({
  selector: 'app-profile-info-field',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="space-y-1">
      <label class="block text-xs font-semibold text-gray-500 uppercase tracking-wide">
        {{ label }}
      </label>
      <div class="flex items-center gap-2">
        <p class="text-sm text-gray-900 font-medium">
          {{ isVisible ? (value || '-') : maskedValue }}
        </p>
        <button
          *ngIf="maskable && value"
          (click)="toggleVisibility()"
          class="text-gray-400 hover:text-gray-600 transition-colors"
          [attr.aria-label]="isVisible ? 'Hide ' + label : 'Show ' + label"
          type="button"
        >
          <svg
            class="w-4 h-4"
            [ngClass]="isVisible ? 'text-blue-600' : 'text-gray-400'"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path
              *ngIf="isVisible"
              d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"
            ></path>
            <circle *ngIf="isVisible" cx="12" cy="12" r="3"></circle>
            <path
              *ngIf="!isVisible"
              d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"
            ></path>
            <line *ngIf="!isVisible" x1="1" y1="1" x2="23" y2="23"></line>
          </svg>
        </button>
      </div>
    </div>
  `,
})
export class ProfileInfoFieldComponent implements OnInit {
  @Input() label: string = '';
  @Input() value: string | null | undefined = '';
  @Input() maskable: boolean = false;
  @Input() maskChar: string = '•';

  isVisible: boolean = false;
  maskedValue: string = '';

  ngOnInit(): void {
    this.updateMaskedValue();
  }

  ngOnChanges(): void {
    this.updateMaskedValue();
  }

  toggleVisibility(): void {
    this.isVisible = !this.isVisible;
  }

  private updateMaskedValue(): void {
    if (this.value && this.maskable) {
      const valueStr = String(this.value);
      const visibleChars = Math.ceil(valueStr.length / 4);
      const hiddenChars = valueStr.length - visibleChars;
      this.maskedValue =
        this.maskChar.repeat(hiddenChars) +
        valueStr.substring(hiddenChars);
    }
  }
}
