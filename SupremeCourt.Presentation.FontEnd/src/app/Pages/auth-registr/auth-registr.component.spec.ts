import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthRegistrComponent } from './auth-registr.component';

describe('AuthRegistrComponent', () => {
  let component: AuthRegistrComponent;
  let fixture: ComponentFixture<AuthRegistrComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuthRegistrComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AuthRegistrComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
