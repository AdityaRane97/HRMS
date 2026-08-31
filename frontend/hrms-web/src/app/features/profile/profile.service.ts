import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_CONFIG } from '@core/config/api.config';

// Demographic Information Interface
// TODO: Once backend is extended with demographic fields, map from EmployeeDto
export interface DemographicInfo {
  dateOfBirth?: string;
  gender?: string; // Male, Female, Other, Prefer Not to Say
  maritalStatus?: string; // Single, Married, Divorced, Widowed
  highestEducation?: string; // High School, Bachelor's, Master's, PhD, etc.
  religion?: string;
  bloodGroup?: string;
}

// National Identifier Interface
// TODO: Backend API needed for national identifiers storage
export interface NationalIdentifier {
  country: string;
  idType: string; // Aadhaar, PAN, Passport, License, etc.
  idNumber: string;
  isPrimary: boolean;
  issueDate?: string;
  expiryDate?: string;
}

// Family/Emergency Contact Interface
// TODO: Backend API needed for emergency contacts storage
export interface FamilyContact {
  id?: string;
  name: string;
  relationship: string; // Spouse, Parent, Sibling, Child, Emergency Contact, etc.
  phoneNumber?: string;
  email?: string;
  dateOfBirth?: string;
  address?: string;
  isEmergencyContact: boolean;
}

export interface EmployeeProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  alternatePhoneNumber?: string;
  dateOfBirth?: string;
  joinDate: string;
  department: string;
  designation: string;
  employeeCode: string;
  employmentStatus: string;
  employmentType: string;
  profilePhotoUrl?: string;

  // Contact Information
  personalEmail?: string;
  workEmail?: string;

  // Address Information
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;

  // Demographic Information
  demographic?: DemographicInfo;

  // National Identifiers
  nationalIdentifiers?: NationalIdentifier[];

  // Family and Emergency Contacts
  familyContacts?: FamilyContact[];
}

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  constructor(private http: HttpClient) {}

  getProfile(): Observable<EmployeeProfile> {
    // TODO: Replace with actual backend API call when available
    return new Observable((observer) => {
      observer.next({
        id: '1',
        firstName: 'Aditya',
        lastName: 'Rane',
        email: 'aditya.rane@example.com',
        phoneNumber: '+91 9876543210',
        alternatePhoneNumber: '+91 9123456789',
        dateOfBirth: '1997-02-15',
        joinDate: '2022-03-01',
        department: 'Engineering',
        designation: 'Senior Software Engineer',
        employeeCode: 'EMP001',
        employmentStatus: 'Active',
        employmentType: 'FullTime',
        profilePhotoUrl: undefined, // Falls back to avatar

        // Contact Information
        personalEmail: 'aditya.personal@example.com',
        workEmail: 'aditya.rane@company.com',

        // Address Information
        address: '123 Tech Street, Tech Park',
        city: 'Bangalore',
        state: 'Karnataka',
        country: 'India',
        postalCode: '560001',

        // Demographic Information
        demographic: {
          dateOfBirth: '1997-02-15',
          gender: 'Male',
          maritalStatus: 'Married',
          highestEducation: "Bachelor's in Computer Science",
          religion: 'Hindu',
          bloodGroup: 'O+',
        },

        // National Identifiers (with mock data)
        nationalIdentifiers: [
          {
            country: 'India',
            idType: 'Aadhaar Number',
            idNumber: '123456789012',
            isPrimary: true,
            issueDate: '2015-03-20',
            expiryDate: undefined,
          },
          {
            country: 'India',
            idType: 'PAN',
            idNumber: 'ABCDE1234F',
            isPrimary: false,
            issueDate: '2010-05-15',
          },
        ],

        // Family and Emergency Contacts
        familyContacts: [
          {
            id: '1',
            name: 'Ritika Rane',
            relationship: 'Spouse',
            phoneNumber: '+91 9876543211',
            email: 'ritika.rane@example.com',
            dateOfBirth: '1998-06-20',
            address: '123 Tech Street, Tech Park, Bangalore',
            isEmergencyContact: true,
          },
          {
            id: '2',
            name: 'Ram Rane',
            relationship: 'Father',
            phoneNumber: '+91 9876543212',
            email: 'ram.rane@example.com',
            dateOfBirth: '1970-01-10',
            address: '456 Family Lane, Pune',
            isEmergencyContact: false,
          },
        ],
      });
      observer.complete();
    });
  }

  updateProfile(profile: EmployeeProfile): Observable<void> {
    // TODO: Implement backend API call when profile update endpoint is available
    // For now, just return success for mock implementation
    return new Observable((observer) => {
      observer.next();
      observer.complete();
    });
  }
}
