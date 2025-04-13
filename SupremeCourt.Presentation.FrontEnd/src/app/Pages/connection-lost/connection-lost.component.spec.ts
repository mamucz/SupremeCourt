import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConnectionLostComponent } from './connection-lost.component';

describe('ConnectionLostComponent', () => {
  let component: ConnectionLostComponent;
  let fixture: ComponentFixture<ConnectionLostComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConnectionLostComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ConnectionLostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
