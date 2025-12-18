import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JobDialog } from './job-dialog';

describe('JobDialog', () => {
  let component: JobDialog;
  let fixture: ComponentFixture<JobDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JobDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JobDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
