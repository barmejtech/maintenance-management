import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MeterReadingsComponent } from './meter-readings';

describe('MeterReadingsComponent', () => {
  let component: MeterReadingsComponent;
  let fixture: ComponentFixture<MeterReadingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MeterReadingsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MeterReadingsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
