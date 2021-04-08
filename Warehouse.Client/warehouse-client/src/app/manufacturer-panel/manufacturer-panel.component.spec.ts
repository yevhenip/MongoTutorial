import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManufacturerPanelComponent } from './manufacturer-panel.component';

describe('ManufacturerPanelComponent', () => {
  let component: ManufacturerPanelComponent;
  let fixture: ComponentFixture<ManufacturerPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ManufacturerPanelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ManufacturerPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
