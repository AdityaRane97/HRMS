import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import {
  ProfileService,
  EmployeeProfile,
  NationalIdentifier,
  FamilyContact,
} from './profile.service';
import { ProfileInfoFieldComponent } from './profile-info-field.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ProfileInfoFieldComponent],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  profileForm!: FormGroup;
  profile: EmployeeProfile | null = null;
  loading = true;
  editing = false;
  saving = false;
  message: string | null = null;

  // Visibility toggles for masked fields
  visibleNationalIds: Set<string> = new Set();
  editingContactId: string | null = null;

  constructor(
    private profileService: ProfileService,
    private formBuilder: FormBuilder,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  private loadProfile(): void {
    this.profileService.getProfile().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.initializeForm();
        this.loading = false;
      },
      error: () => {
        this.message = 'Failed to load profile';
        this.loading = false;
      },
    });
  }

  private initializeForm(): void {
    if (this.profile) {
      this.profileForm = this.formBuilder.group({
        firstName: [this.profile.firstName, Validators.required],
        lastName: [this.profile.lastName, Validators.required],
        email: [{ value: this.profile.email, disabled: true }, Validators.required],
        phoneNumber: [this.profile.phoneNumber || '', Validators.required],
        dateOfBirth: [this.profile.dateOfBirth || ''],
        department: [{ value: this.profile.department, disabled: true }],
        designation: [{ value: this.profile.designation, disabled: true }],
      });
    }
  }

  toggleEdit(): void {
    this.editing = !this.editing;
    if (!this.editing) {
      this.initializeForm();
    }
  }

  onSave(): void {
    if (this.profileForm.invalid) {
      return;
    }

    this.saving = true;
    const updatedProfile: EmployeeProfile = {
      ...this.profile!,
      ...this.profileForm.getRawValue(),
    };

    this.profileService.updateProfile(updatedProfile).subscribe({
      next: () => {
        this.profile = updatedProfile;
        this.editing = false;
        this.message = 'Profile updated successfully';
        this.saving = false;
        setTimeout(() => (this.message = null), 3000);
      },
      error: () => {
        this.message = 'Failed to update profile. Please try again.';
        this.saving = false;
        setTimeout(() => (this.message = null), 3000);
      },
    });
  }

  // Helper methods for display
  goBack(): void {
    this.location.back();
  }

  getInitials(): string {
    if (!this.profile) return '';
    const firstInitial = this.profile.firstName?.charAt(0) || '';
    const lastInitial = this.profile.lastName?.charAt(0) || '';
    return (firstInitial + lastInitial).toUpperCase();
  }

  getFullName(): string {
    if (!this.profile) return '';
    return `${this.profile.firstName} ${this.profile.lastName}`.trim();
  }

  getDisplayValue(value: string | null | undefined): string {
    return value && value.trim() !== '' ? value : '-';
  }

  toggleNationalIdVisibility(id?: string): void {
    if (!id) return;
    if (this.visibleNationalIds.has(id)) {
      this.visibleNationalIds.delete(id);
    } else {
      this.visibleNationalIds.add(id);
    }
  }

  isNationalIdVisible(id?: string): boolean {
    return id ? this.visibleNationalIds.has(id) : false;
  }

  getMaskedIdNumber(idNumber: string): string {
    if (!idNumber) return '-';
    const visibleChars = Math.ceil(idNumber.length / 4);
    const hiddenChars = idNumber.length - visibleChars;
    return '•'.repeat(hiddenChars) + idNumber.substring(hiddenChars);
  }

  // Family contact helpers
  editFamilyContact(id?: string): void {
    this.editingContactId = id || null;
    // TODO: Implement full edit mode for family contacts
  }

  deleteFamilyContact(id?: string): void {
    if (!id || !this.profile?.familyContacts) return;
    // TODO: Implement backend deletion when API is available
    this.profile.familyContacts = this.profile.familyContacts.filter(c => c.id !== id);
    this.message = 'Contact deleted';
    setTimeout(() => (this.message = null), 2000);
  }

  addFamilyContact(): void {
    // TODO: Open modal/form to add new family contact
    this.message = 'Add contact feature coming soon';
    setTimeout(() => (this.message = null), 2000);
  }

  formatDate(dateString?: string): string {
    if (!dateString) return '-';
    try {
      const date = new Date(dateString);
      return date.toLocaleDateString('en-IN', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      });
    } catch {
      return dateString;
    }
  }
}

