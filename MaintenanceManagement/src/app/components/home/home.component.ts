import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements AfterViewInit, OnDestroy {
  currentSlide = 0;
  readonly currentYear = new Date().getFullYear();
  private slideInterval: ReturnType<typeof setInterval> | null = null;

  readonly slides = [
    {
      image: 'carousel-1.svg',
      title: 'Smart Maintenance Management',
      subtitle: 'Streamline your facility operations with real-time work order tracking and automated scheduling.'
    },
    {
      image: 'carousel-2.svg',
      title: 'Preventive Maintenance Scheduling',
      subtitle: 'Schedule, track and complete maintenance tasks before equipment failures occur.'
    },
    {
      image: 'carousel-3.svg',
      title: 'Analytics & Real-Time Insights',
      subtitle: 'Comprehensive dashboards showing KPIs, trends and team performance at a glance.'
    }
  ];

  readonly features = [
    {
      icon: 'bi-clipboard2-check',
      title: 'Work Order Management',
      description: 'Create, assign and track work orders with priorities, due dates and real-time status updates.'
    },
    {
      icon: 'bi-people',
      title: 'Technician Coordination',
      description: 'Manage your maintenance team, track availability and assign tasks to the right people.'
    },
    {
      icon: 'bi-gear',
      title: 'Equipment Tracking',
      description: 'Keep a full inventory of all assets, maintenance history and upcoming service schedules.'
    },
    {
      icon: 'bi-bell',
      title: 'Real-Time Notifications',
      description: 'Instant alerts via SignalR when tasks are assigned, updated or completed.'
    },
    {
      icon: 'bi-bar-chart-line',
      title: 'Reports & Analytics',
      description: 'Generate detailed maintenance reports with charts, cost breakdowns and KPI dashboards.'
    },
    {
      icon: 'bi-chat-dots',
      title: 'Team Collaboration',
      description: 'Built-in chat so technicians and managers can communicate without leaving the platform.'
    }
  ];

  readonly testimonials = [
    {
      name: 'Ahmed Benali',
      role: 'Facility Manager',
      company: 'TechCorp Industries',
      text: 'This platform cut our equipment downtime by 40%. The preventive scheduling and real-time notifications are game changers.',
      avatar: 'AB'
    },
    {
      name: 'Sarah Johnson',
      role: 'Maintenance Supervisor',
      company: 'BuildRight Corp',
      text: 'Finally a system that keeps our entire team in sync. The mobile-friendly dashboard is exactly what we needed.',
      avatar: 'SJ'
    },
    {
      name: 'Mohamed Oussama',
      role: 'Operations Director',
      company: 'AlgerPro',
      text: 'The analytics dashboards give me instant visibility into my entire fleet. ROI was positive within 3 months.',
      avatar: 'MO'
    }
  ];

  ngAfterViewInit(): void {
    this.slideInterval = setInterval(() => {
      this.nextSlide();
    }, 5000);
  }

  ngOnDestroy(): void {
    if (this.slideInterval) clearInterval(this.slideInterval);
  }

  goToSlide(index: number): void {
    this.currentSlide = index;
  }

  prevSlide(): void {
    this.currentSlide = (this.currentSlide - 1 + this.slides.length) % this.slides.length;
  }

  nextSlide(): void {
    this.currentSlide = (this.currentSlide + 1) % this.slides.length;
  }
}
