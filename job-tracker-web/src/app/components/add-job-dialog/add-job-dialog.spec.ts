import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddJobDialog } from './add-job-dialog';

describe('AddJobDialog', () => {
  let component: AddJobDialog;
  let fixture: ComponentFixture<AddJobDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddJobDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddJobDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
