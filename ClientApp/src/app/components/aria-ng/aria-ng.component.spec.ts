import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AriaNgComponent } from './aria-ng.component';

describe('AriaNgComponent', () => {
  let component: AriaNgComponent;
  let fixture: ComponentFixture<AriaNgComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AriaNgComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AriaNgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
