import { Routes } from '@angular/router';
import { authGuard, guestGuard, adminGuard, managerGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'home',
    loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent)
  },
  {
    path: 'login',
    loadComponent: () => import('./components/auth/login.component').then(m => m.LoginComponent),
    canActivate: [guestGuard]
  },
  {
    path: 'register',
    loadComponent: () => import('./components/auth/register.component').then(m => m.RegisterComponent),
    canActivate: [authGuard, adminGuard]
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'technicians',
    loadComponent: () => import('./components/technicians/technicians.component').then(m => m.TechniciansComponent),
    canActivate: [authGuard, managerGuard]
  },
  {
    path: 'tasks',
    loadComponent: () => import('./components/tasks/tasks.component').then(m => m.TasksComponent),
    canActivate: [authGuard]
  },
  {
    path: 'groups',
    loadComponent: () => import('./components/groups/groups.component').then(m => m.GroupsComponent),
    canActivate: [authGuard, managerGuard]
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
    canActivate: [authGuard, managerGuard]
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
  {
    path: 'predictive-maintenance',
    loadComponent: () => import('./components/predictive-maintenance/predictive-maintenance.component').then(m => m.PredictiveMaintenanceComponent),
    canActivate: [authGuard]
  },
  {
    path: 'equipment-digital-twin',
    loadComponent: () => import('./components/equipment-digital-twin/equipment-digital-twin.component').then(m => m.EquipmentDigitalTwinComponent),
    canActivate: [authGuard]
  },
  {
    path: 'performance',
    loadComponent: () => import('./components/performance/performance.component').then(m => m.PerformanceComponent),
    canActivate: [authGuard, managerGuard]
  },
  {
    path: 'spare-parts',
    loadComponent: () => import('./components/spare-parts/spare-parts.component').then(m => m.SparePartsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'maintenance-schedules',
    loadComponent: () => import('./components/maintenance-schedules/maintenance-schedules.component').then(m => m.MaintenanceSchedulesComponent),
    canActivate: [authGuard]
  },
  {
    path: 'equipment-timeline',
    loadComponent: () => import('./components/equipment-timeline/equipment-timeline.component').then(m => m.EquipmentTimelineComponent),
    canActivate: [authGuard]
  },
  {
    path: 'mobile-workflow',
    loadComponent: () => import('./components/mobile-workflow/mobile-workflow.component').then(m => m.MobileWorkflowComponent),
    canActivate: [authGuard]
  },
  { path: '**', redirectTo: '/dashboard' }
];


