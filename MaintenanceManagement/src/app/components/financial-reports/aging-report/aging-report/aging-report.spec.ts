import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgingReportComponent } from './aging-report';

describe('AgingReportComponent', () => {
  let component: AgingReportComponent;
  let fixture: ComponentFixture<AgingReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AgingReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AgingReportComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
