import { Routes } from '@angular/router';
import { authGuard, guestGuard, adminGuard, managerGuard, clientGuard, nonClientGuard } from './guards/auth.guard';

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
    path: 'client-register',
    loadComponent: () => import('./components/auth/client-register.component').then(m => m.ClientRegisterComponent)
  },
  // ── Client Portal ───────────────────────────────────────────────────────────
  {
    path: 'client-portal',
    loadComponent: () => import('./components/client-portal/client-portal.component').then(m => m.ClientPortalComponent),
    canActivate: [authGuard, clientGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadComponent: () => import('./components/client-portal/client-dashboard.component').then(m => m.ClientDashboardComponent)
      },
      {
        path: 'my-requests',
        loadComponent: () => import('./components/client-portal/client-requests.component').then(m => m.ClientRequestsComponent)
      },
      {
        path: 'new-request',
        loadComponent: () => import('./components/client-portal/new-request.component').then(m => m.NewRequestComponent)
      },
      {
        path: 'chat',
        loadComponent: () => import('./components/chat/chat.component').then(m => m.ChatComponent)
      },
      {
        path: 'notifications',
        loadComponent: () => import('./components/notifications-page/notifications-page.component').then(m => m.NotificationsPageComponent)
      },
      {
        path: 'profile',
        loadComponent: () => import('./components/client-portal/client-profile.component').then(m => m.ClientProfileComponent)
      }
    ]
  },
  // ── Internal Portal (non-client users) ──────────────────────────────────────
  {
    path: '',
    loadComponent: () => import('./components/shell/shell.component').then(m => m.ShellComponent),
    canActivate: [authGuard, nonClientGuard],
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
        path: 'data-entry',
        loadComponent: () => import('./components/data-entry/data-entry.component').then(m => m.DataEntryComponent),
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
        path: 'clients',
        loadComponent: () => import('./components/clients/clients.component').then(m => m.ClientsComponent),
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
        loadComponent: () => import('./components/notifications-page/notifications-page.component').then(m => m.NotificationsPageComponent)
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
      },
      {
        path: 'power-bi',
        loadComponent: () => import('./components/power-bi/power-bi.component').then(m => m.PowerBiComponent)
      },
      {
        path: 'account',
        loadComponent: () => import('./components/account/account.component').then(m => m.AccountComponent)
      },
      {
        path: 'requests',
        loadComponent: () => import('./components/requests/requests.component').then(m => m.RequestsComponent),
        canActivate: [managerGuard]
      },
      {
        path: 'premium-maintenance',
        loadComponent: () => import('./components/premium-maintenance/premium-maintenance.component').then(m => m.PremiumMaintenanceComponent)
      },
      {
        path: 'admin-dashboard',
        loadComponent: () => import('./components/admin-dashboard/admin-dashboard.component').then(m => m.AdminDashboardComponent),
        canActivate: [adminGuard]
      }
    ]
  },
  { path: '**', redirectTo: '/dashboard' }
];


