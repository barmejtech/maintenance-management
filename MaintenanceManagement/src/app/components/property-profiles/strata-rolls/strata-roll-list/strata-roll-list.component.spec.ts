import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StrataRollListComponent } from './strata-roll-list.component';

describe('StrataRollListComponent', () => {
  let component: StrataRollListComponent;
  let fixture: ComponentFixture<StrataRollListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StrataRollListComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(StrataRollListComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
