import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountsGL } from './accounts-gl';

describe('AccountsGL', () => {
  let component: AccountsGL;
  let fixture: ComponentFixture<AccountsGL>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountsGL]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountsGL);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
