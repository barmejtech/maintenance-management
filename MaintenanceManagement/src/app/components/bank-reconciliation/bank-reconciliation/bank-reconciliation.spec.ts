import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BankReconciliationComponent } from './bank-reconciliation';

describe('BankReconciliationComponent', () => {
  let component: BankReconciliationComponent;
  let fixture: ComponentFixture<BankReconciliationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BankReconciliationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BankReconciliationComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
