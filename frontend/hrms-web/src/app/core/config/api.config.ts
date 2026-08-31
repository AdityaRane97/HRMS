import { environment } from '@environments/environment';

export const API_CONFIG = {
  baseUrl: environment.apiUrl,
  endpoints: {
    auth: {
      login: `${environment.apiEndpoints.auth}/login`,
      logout: `${environment.apiEndpoints.auth}/logout`,
      profile: `${environment.apiEndpoints.auth}/profile`,
    },
    // TODO: Add endpoints for other features
    dashboard: '/api/dashboard',
    profile: '/api/profile',
    team: '/api/team',
    salary: '/api/salary',
    timesheet: '/api/timesheet',
    documents: '/api/documents',
    policies: '/api/policies',
    rewards: '/api/rewards',
    tasks: '/api/tasks',
    administration: '/api/administration',
  },
};
