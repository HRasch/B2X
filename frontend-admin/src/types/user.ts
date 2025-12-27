// User types for admin
export interface User {
  id: string;
  tenantId: string;
  email: string;
  phoneNumber?: string;
  firstName: string;
  lastName: string;
  isEmailVerified: boolean;
  isPhoneVerified: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  lastLoginAt?: string;
  createdBy?: string;
  updatedBy?: string;
}

export interface UserProfile {
  id: string;
  userId: string;
  tenantId: string;
  avatarUrl?: string;
  bio?: string;
  dateOfBirth?: string;
  gender?: string;
  nationality?: string;
  companyName?: string;
  jobTitle?: string;
  preferredLanguage?: string;
  timezone?: string;
  receiveNewsletter: boolean;
  receivePromotionalEmails: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface Address {
  id: string;
  userId: string;
  tenantId: string;
  addressType: string;
  streetAddress: string;
  streetAddress2?: string;
  city: string;
  postalCode: string;
  country: string;
  state?: string;
  recipientName: string;
  phoneNumber?: string;
  isDefault: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface UsersListResponse {
  data: User[];
  pagination: {
    page: number;
    pageSize: number;
    total: number;
    hasNext: boolean;
    hasPrevious: boolean;
  };
}
