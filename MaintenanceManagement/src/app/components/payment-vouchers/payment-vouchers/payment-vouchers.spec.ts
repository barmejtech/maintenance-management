import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentVouchersComponent } from './payment-vouchers';

describe('PaymentVouchersComponent', () => {
  let component: PaymentVouchersComponent;
  let fixture: ComponentFixture<PaymentVouchersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaymentVouchersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PaymentVouchersComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
