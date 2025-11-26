import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnalysisDialog } from './analysis-dialog';

describe('AnalysisDialog', () => {
  let component: AnalysisDialog;
  let fixture: ComponentFixture<AnalysisDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AnalysisDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AnalysisDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
