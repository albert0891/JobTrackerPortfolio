import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { provideNativeDateAdapter } from '@angular/material/core';

import { JobService } from '../../services/job.service';
import { JobApplication } from '../../models/job-application.model';

@Component({
  selector: 'app-add-job-dialog',
  standalone: true,
  providers: [provideNativeDateAdapter()], // For date picker
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
  ],
  templateUrl: './add-job-dialog.html',
  styleUrls: ['./add-job-dialog.scss'],
})
export class AddJobDialogComponent {
  private fb = inject(FormBuilder);
  private jobService = inject(JobService);
  public dialogRef = inject(MatDialogRef<AddJobDialogComponent>);

  public jobForm: FormGroup = this.fb.group({
    jobTitle: ['', Validators.required],
    companyName: ['', Validators.required],
    jobDescription: [''],
    status: ['Applied', Validators.required],
    dateApplied: [new Date(), Validators.required],
  });

  public readonly statuses = ['Applied', 'Interviewing', 'Offer', 'Rejected'];

  onSubmit(): void {
    if (this.jobForm.valid) {
      const newJob: JobApplication = this.jobForm.value;

      // Call service to create job
      this.jobService.addJob(newJob).subscribe({
        next: (createdJob) => {
          console.log('Job created:', createdJob);
          this.dialogRef.close(true); // Close and signal success
        },
        error: (err) => {
          console.error('Failed to create job:', err);
          // In a real app, show a snackbar notification here
        },
      });
    }
  }

  close(): void {
    this.dialogRef.close(false);
  }
}
