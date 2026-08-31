export interface LoginRequest {
  username: string;
  password: string;
  ipAddress?: string;
  userAgent?: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
  employee: {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    role: string;
  };
}
