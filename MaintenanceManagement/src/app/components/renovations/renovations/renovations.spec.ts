import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RenovationsComponent } from './renovations';

describe('RenovationsComponent', () => {
  let component: RenovationsComponent;
  let fixture: ComponentFixture<RenovationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RenovationsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RenovationsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
