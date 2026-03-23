import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translate.service';
import { TranslatePipe } from '../../pipes/translate.pipe';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements AfterViewInit, OnDestroy {
  constructor(public auth: AuthService, public translation: TranslationService) {}
  currentSlide = 0;
  readonly currentYear = new Date().getFullYear();
  private slideInterval: ReturnType<typeof setInterval> | null = null;

  readonly slides = [
    {
      image: 'carousel-1.svg',
      titleKey: 'home.slides.slide1.title',
      subtitleKey: 'home.slides.slide1.subtitle'
    },
    {
      image: 'carousel-2.svg',
      titleKey: 'home.slides.slide2.title',
      subtitleKey: 'home.slides.slide2.subtitle'
    },
    {
      image: 'carousel-3.svg',
      titleKey: 'home.slides.slide3.title',
      subtitleKey: 'home.slides.slide3.subtitle'
    }
  ];

  readonly features = [
    { icon: 'bi-clipboard2-check', titleKey: 'home.features.workOrder.title', descriptionKey: 'home.features.workOrder.description' },
    { icon: 'bi-people',           titleKey: 'home.features.technician.title', descriptionKey: 'home.features.technician.description' },
    { icon: 'bi-gear',             titleKey: 'home.features.equipment.title', descriptionKey: 'home.features.equipment.description' },
    { icon: 'bi-bell',             titleKey: 'home.features.notifications.title', descriptionKey: 'home.features.notifications.description' },
    { icon: 'bi-bar-chart-line',   titleKey: 'home.features.reports.title', descriptionKey: 'home.features.reports.description' },
    { icon: 'bi-chat-dots',        titleKey: 'home.features.collaboration.title', descriptionKey: 'home.features.collaboration.description' }
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
