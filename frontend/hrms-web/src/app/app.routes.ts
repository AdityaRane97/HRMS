import { Routes } from '@angular/router';
import { AuthGuard } from '@core/auth/auth.guard';
import { AuthLayoutComponent } from '@layout/auth-layout/auth-layout.component';
import { MainLayoutComponent } from '@layout/main-layout/main-layout.component';
import { LoginComponent } from '@features/auth/login/login.component';
import { DashboardComponent } from '@features/dashboard/dashboard.component';
import { ProfileComponent } from '@features/profile/profile.component';
import { TeamComponent } from '@features/team/team.component';
import { SalaryComponent } from '@features/salary/salary.component';
import { PayslipComponent } from '@features/salary/payslip/payslip.component';
import { YTDReportsComponent } from '@features/salary/ytd-reports/ytd-reports.component';
import { ITDeclarationComponent } from '@features/salary/declarations/it-declaration.component';
import { FBPDeclarationComponent } from '@features/salary/declarations/fbp-declaration.component';
import { TimesheetComponent } from '@features/timesheet/timesheet.component';
import { CurrentTimeCardComponent } from '@features/timesheet/current-time-card/current-time-card.component';
import { ExistingTimeCardsComponent } from '@features/timesheet/existing-time-cards/existing-time-cards.component';
import { AddAbsenceComponent } from '@features/timesheet/add-absence/add-absence.component';
import { ExistingAbsencesComponent } from '@features/timesheet/existing-absences/existing-absences.component';
import { AbsenceBalanceComponent } from '@features/timesheet/absence-balance/absence-balance.component';
import { TeamScheduleComponent } from '@features/timesheet/team-schedule/team-schedule.component';
import { DocumentsComponent } from '@features/documents/documents.component';
import { PoliciesComponent } from '@features/policies/policies.component';
import { RewardsComponent } from '@features/rewards/rewards.component';
import { TasksComponent } from '@features/tasks/tasks.component';
import { TaskListComponent } from '@features/tasks/task-list/task-list.component';
import { TaskDetailComponent } from '@features/tasks/task-detail/task-detail.component';
import { CreateTaskComponent } from '@features/tasks/create-task/create-task.component';
import { EditTaskComponent } from '@features/tasks/edit-task/edit-task.component';
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
      { 
        path: 'salary', 
        component: SalaryComponent,
      },
      {
        path: 'salary/payslip',
        component: PayslipComponent,
      },
      {
        path: 'salary/ytd-reports',
        component: YTDReportsComponent,
      },
      {
        path: 'salary/it-declaration',
        component: ITDeclarationComponent,
      },
      {
        path: 'salary/fbp-declaration',
        component: FBPDeclarationComponent,
      },
      {
        path: 'timesheet',
        component: TimesheetComponent,
      },
      {
        path: 'timesheet/current-time-card',
        component: CurrentTimeCardComponent,
      },
      {
        path: 'timesheet/existing-time-cards',
        component: ExistingTimeCardsComponent,
      },
      {
        path: 'timesheet/add-absence',
        component: AddAbsenceComponent,
      },
      {
        path: 'timesheet/existing-absences',
        component: ExistingAbsencesComponent,
      },
      {
        path: 'timesheet/absence-balance',
        component: AbsenceBalanceComponent,
      },
      {
        path: 'timesheet/team-schedule',
        component: TeamScheduleComponent,
      },
      { path: 'documents', component: DocumentsComponent },
      { path: 'policies', component: PoliciesComponent },
      { path: 'rewards', component: RewardsComponent },
      {
        path: 'tasks',
        component: TasksComponent,
      },
      {
        path: 'tasks/list',
        component: TaskListComponent,
      },
      {
        path: 'tasks/create',
        component: CreateTaskComponent,
      },
      {
        path: 'tasks/:id',
        component: TaskDetailComponent,
      },
      {
        path: 'tasks/:id/edit',
        component: EditTaskComponent,
      },
      { path: 'chatbot', component: ChatbotComponent },
      { path: 'administration', component: AdministrationComponent },
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'auth/login' },
];
