import { Routes } from '@angular/router';
import { AuthGuard } from '@core/auth/auth.guard';
import { AuthLayoutComponent } from '@layout/auth-layout/auth-layout.component';
import { MainLayoutComponent } from '@layout/main-layout/main-layout.component';
import { LoginComponent } from '@features/auth/login/login.component';
import { DashboardComponent } from '@features/dashboard/dashboard.component';
import { ProfileComponent } from '@features/profile/profile.component';
import { TeamComponent } from '@features/team/team.component';
import { SalaryComponent } from '@features/salary/salary.component';
import { TimesheetComponent } from '@features/timesheet/timesheet.component';
import { DocumentsComponent } from '@features/documents/documents.component';
import { PoliciesComponent } from '@features/policies/policies.component';
import { RewardsComponent } from '@features/rewards/rewards.component';
import { TasksComponent } from '@features/tasks/tasks.component';
import { ChatbotComponent } from '@features/chatbot/chatbot.component';
import { AdministrationComponent } from '@features/administration/administration.component';

export const routes: Routes = [
  {
    path: 'auth',
    component: AuthLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: '', redirectTo: 'login', pathMatch: 'full' },
    ],
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'dashboard', component: DashboardComponent },
      { path: 'profile', component: ProfileComponent },
      { path: 'team', component: TeamComponent },
      { path: 'salary', component: SalaryComponent },
      { path: 'timesheet', component: TimesheetComponent },
      { path: 'documents', component: DocumentsComponent },
      { path: 'policies', component: PoliciesComponent },
      { path: 'rewards', component: RewardsComponent },
      { path: 'tasks', component: TasksComponent },
      { path: 'chatbot', component: ChatbotComponent },
      { path: 'administration', component: AdministrationComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'auth/login' },
];
