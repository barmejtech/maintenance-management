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
    path: '',
    loadComponent: () => import('./components/shell/shell.component').then(m => m.ShellComponent),
    canActivate: [authGuard],
    children: [
      {
        path: 'register',
        loadComponent: () => import('./components/auth/register.component').then(m => m.RegisterComponent),
        canActivate: [adminGuard]
      },
      {
        path: 'dashboard',
        loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'managers',
        loadComponent: () => import('./components/managers/managers.component').then(m => m.ManagersComponent),
        canActivate: [adminGuard]
      },
      {
        path: 'users',
        loadComponent: () => import('./components/users/users.component').then(m => m.UsersComponent),
        canActivate: [adminGuard]
      },
      {
        path: 'technicians',
        loadComponent: () => import('./components/technicians/technicians.component').then(m => m.TechniciansComponent),
        canActivate: [managerGuard]
      },
      {
        path: 'tasks',
        loadComponent: () => import('./components/tasks/tasks.component').then(m => m.TasksComponent)
      },
      {
        path: 'groups',
        loadComponent: () => import('./components/groups/groups.component').then(m => m.GroupsComponent),
        canActivate: [managerGuard]
      },
      {
        path: 'equipment',
        loadComponent: () => import('./components/equipment/equipment.component').then(m => m.EquipmentComponent)
      },
      {
        path: 'hvac',
        loadComponent: () => import('./components/hvac/hvac.component').then(m => m.HVACComponent)
      },
      {
        path: 'reports',
        loadComponent: () => import('./components/reports/reports.component').then(m => m.ReportsComponent)
      },
      {
        path: 'invoices',
        loadComponent: () => import('./components/invoices/invoices.component').then(m => m.InvoicesComponent),
        canActivate: [managerGuard]
      },
      {
        path: 'availability',
        loadComponent: () => import('./components/availability/availability.component').then(m => m.AvailabilityComponent)
      },
      {
        path: 'chat',
        loadComponent: () => import('./components/chat/chat.component').then(m => m.ChatComponent)
      },
      {
        path: 'calendar',
        loadComponent: () => import('./components/calendar/calendar.component').then(m => m.CalendarComponent)
      },
      {
        path: 'notifications',
        loadComponent: () => import('./components/notifications/notifications.component').then(m => m.NotificationsComponent)
      },
      {
        path: 'predictive-maintenance',
        loadComponent: () => import('./components/predictive-maintenance/predictive-maintenance.component').then(m => m.PredictiveMaintenanceComponent)
      },
      {
        path: 'equipment-digital-twin',
        loadComponent: () => import('./components/equipment-digital-twin/equipment-digital-twin.component').then(m => m.EquipmentDigitalTwinComponent)
      },
      {
        path: 'performance',
        loadComponent: () => import('./components/performance/performance.component').then(m => m.PerformanceComponent),
        canActivate: [managerGuard]
      },
      {
        path: 'spare-parts',
        loadComponent: () => import('./components/spare-parts/spare-parts.component').then(m => m.SparePartsComponent)
      },
      {
        path: 'maintenance-schedules',
        loadComponent: () => import('./components/maintenance-schedules/maintenance-schedules.component').then(m => m.MaintenanceSchedulesComponent)
      },
      {
        path: 'equipment-timeline',
        loadComponent: () => import('./components/equipment-timeline/equipment-timeline.component').then(m => m.EquipmentTimelineComponent)
      },
      {
        path: 'mobile-workflow',
        loadComponent: () => import('./components/mobile-workflow/mobile-workflow.component').then(m => m.MobileWorkflowComponent)
      }
    ]
  },
  { path: '**', redirectTo: '/dashboard' }
];


