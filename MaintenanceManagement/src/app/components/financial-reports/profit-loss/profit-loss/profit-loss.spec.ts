import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfitLossComponent } from './profit-loss';

describe('ProfitLossComponent', () => {
  let component: ProfitLossComponent;
  let fixture: ComponentFixture<ProfitLossComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfitLossComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProfitLossComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
