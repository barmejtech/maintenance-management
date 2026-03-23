import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./components/auth/login.component').then(m => m.LoginComponent),
    canActivate: [guestGuard]
  },
  {
    path: 'register',
    loadComponent: () => import('./components/auth/register.component').then(m => m.RegisterComponent),
    canActivate: [guestGuard]
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'technicians',
    loadComponent: () => import('./components/technicians/technicians.component').then(m => m.TechniciansComponent),
    canActivate: [authGuard]
  },
  {
    path: 'tasks',
    loadComponent: () => import('./components/tasks/tasks.component').then(m => m.TasksComponent),
    canActivate: [authGuard]
  },
  {
    path: 'groups',
    loadComponent: () => import('./components/groups/groups.component').then(m => m.GroupsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'equipment',
    loadComponent: () => import('./components/equipment/equipment.component').then(m => m.EquipmentComponent),
    canActivate: [authGuard]
  },
  {
    path: 'hvac',
    loadComponent: () => import('./components/hvac/hvac.component').then(m => m.HVACComponent),
    canActivate: [authGuard]
  },
  {
    path: 'reports',
    loadComponent: () => import('./components/reports/reports.component').then(m => m.ReportsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'invoices',
    loadComponent: () => import('./components/invoices/invoices.component').then(m => m.InvoicesComponent),
    canActivate: [authGuard]
  },
  {
    path: 'availability',
    loadComponent: () => import('./components/availability/availability.component').then(m => m.AvailabilityComponent),
    canActivate: [authGuard]
  },
  {
    path: 'chat',
    loadComponent: () => import('./components/chat/chat.component').then(m => m.ChatComponent),
    canActivate: [authGuard]
  },
  {
    path: 'calendar',
    loadComponent: () => import('./components/calendar/calendar.component').then(m => m.CalendarComponent),
    canActivate: [authGuard]
  },
  {
    path: 'notifications',
    loadComponent: () => import('./components/notifications/notifications.component').then(m => m.NotificationsComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/dashboard' }
];


