import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AuditViewerComponent } from './audit-viewer.component';

describe('AuditViewerComponent', () => {
  let component: AuditViewerComponent;
  let fixture: ComponentFixture<AuditViewerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AuditViewerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AuditViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
