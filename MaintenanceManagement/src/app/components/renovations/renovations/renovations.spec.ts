import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Renovations } from './renovations';

describe('Renovations', () => {
  let component: Renovations;
  let fixture: ComponentFixture<Renovations>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Renovations]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Renovations);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
