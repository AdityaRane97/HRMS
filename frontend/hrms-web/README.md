# HRMS Frontend - Angular 22 Application

This is the Angular frontend application for the HRMS (Human Resource Management System).

## Project Structure

```
src/
├── app/
│   ├── core/              # Core services, guards, interceptors
│   │   ├── auth/          # Authentication services and guards
│   │   ├── http/          # HTTP interceptors
│   │   ├── config/        # API configuration
│   │   └── models/        # Core data models
│   ├── shared/            # Shared components and utilities
│   ├── layout/            # Layout components (header, sidebar, footer)
│   ├── features/          # Feature modules
│   │   ├── auth/          # Authentication (login)
│   │   ├── dashboard/     # Dashboard (role-based)
│   │   ├── profile/       # Employee profile
│   │   ├── team/          # Team management
│   │   ├── salary/        # Salary information
│   │   ├── timesheet/     # Timesheet tracking
│   │   ├── documents/     # Document management
│   │   ├── policies/      # Company policies
│   │   ├── rewards/       # Rewards & recognition
│   │   ├── tasks/         # Task management
│   │   ├── chatbot/       # HR Chatbot
│   │   └── administration/# Admin panel
│   ├── app.routes.ts      # Application routing
│   ├── app.config.ts      # Application configuration
│   └── app.component.ts   # Root component
├── styles/                # Global styles and Tailwind CSS
└── environments/          # Environment-specific configurations
```

## Installation & Setup

### Prerequisites

- Node.js 24 LTS (or compatible LTS version)
- npm 10+
- Angular CLI 22.x

### Install Dependencies

```bash
npm install
```

### Running the Development Server

```bash
npm start
```

The application will be available at `http://localhost:4200`.

### API Configuration

The frontend connects to the backend API at `http://localhost:5000` by default.
To change this, edit `src/environments/environment.ts`.

## Authentication

- The application uses JWT-based authentication via httpOnly cookies
- Login endpoint: `POST /api/auth/login`
- Protected routes use `AuthGuard` and require authentication
- Role-based access is enforced via `RoleGuard`

## Building for Production

```bash
npm run build
```

The production build will be available in `dist/hrms-web/`.

## Testing

```bash
npm test
```

## Key Features

- **Role-Based Dashboard**: Employee, Manager, and Admin dashboards
- **Responsive Design**: Mobile, tablet, and desktop support
- **Tailwind CSS**: Utility-first CSS framework
- **Angular Signals**: Modern state management patterns
- **Standalone Components**: Modern Angular architecture
- **Lazy Loading**: Feature modules loaded on demand
- **HTTP Interceptors**: Automatic token handling and error management

## TODO Items

- Implement shared component library (buttons, tables, modals, etc.)
- Complete feature services with backend API calls
- Add unit tests for components and services
- Implement accessibility features (WCAG 2.1 AA)
- Add E2E tests using Cypress or Playwright
- Performance optimization and bundle analysis
- Add internationalization (i18n) support if needed

## Support

For issues or questions, please refer to the project documentation or contact the development team.
