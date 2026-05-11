import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BankReconciliation } from './bank-reconciliation';

describe('BankReconciliation', () => {
  let component: BankReconciliation;
  let fixture: ComponentFixture<BankReconciliation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BankReconciliation]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BankReconciliation);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
