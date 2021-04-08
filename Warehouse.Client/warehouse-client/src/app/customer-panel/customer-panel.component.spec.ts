import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerPanelComponent } from './customer-panel.component';

describe('CustomerPanelComponent', () => {
  let component: CustomerPanelComponent;
  let fixture: ComponentFixture<CustomerPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CustomerPanelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
