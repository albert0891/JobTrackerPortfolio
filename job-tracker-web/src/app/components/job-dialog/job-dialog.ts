import { Component, inject, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';

import { JobService } from '../../services/job.service';
import { JobApplication } from '../../models/job-application.model';

@Component({
  selector: 'app-job-dialog',
  standalone: true,
  providers: [provideNativeDateAdapter()],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
    MatIconModule,
  ],
  templateUrl: './job-dialog.html',
  styleUrls: ['./job-dialog.scss'],
})
export class JobDialogComponent {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);
  public dialogRef = inject(MatDialogRef<JobDialogComponent>);

  public jobForm: FormGroup;
  public isEditMode: boolean = false;
  public readonly statuses = ['Applied', 'Interviewing', 'Offer', 'Rejected'];

  constructor(@Inject(MAT_DIALOG_DATA) public data: { job?: JobApplication }) {
    this.isEditMode = !!data?.job;

    this.jobForm = this.fb.group({
      id: [data?.job?.id || null], // Keep ID for updates
      jobTitle: [data?.job?.jobTitle || '', Validators.required],
      companyName: [data?.job?.companyName || '', Validators.required],
      jobDescription: [data?.job?.jobDescription || ''],
      status: [data?.job?.status || 'Applied', Validators.required],
      dateApplied: [data?.job?.dateApplied || new Date(), Validators.required],
    });
  }

  onSubmit(): void {
    if (this.jobForm.valid) {
      const formValue: JobApplication = this.jobForm.value;

      if (this.isEditMode) {
        this.jobService.updateJob(formValue).subscribe({
          next: () => this.dialogRef.close(true),
          error: (err) => console.error('Update failed', err),
        });
      } else {
        // Remove id from payload entirely so backend handles generation (avoids null to int error)
        const { id, ...newJob } = formValue;
        this.jobService.addJob(newJob as JobApplication).subscribe({
          next: () => this.dialogRef.close(true),
          error: (err) => console.error('Create failed', err),
        });
      }
    }
  }

  onDelete(): void {
    if (this.isEditMode && this.data.job?.id) {
      if (confirm('Are you sure you want to delete this job?')) {
        this.jobService.deleteJob(this.data.job.id).subscribe({
          next: () => this.dialogRef.close(true),
        });
      }
    }
  }

  close(): void {
    this.dialogRef.close(false);
  }
}
