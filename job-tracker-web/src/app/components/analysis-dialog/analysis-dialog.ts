import { Component, Inject, inject, signal, computed, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms'; // For ngModel

import { JobService } from '../../services/job.service';
import { SignalRService } from '../../services/signalr.service';
import { AiAnalysisResult } from '../../models/job-application.model';

@Component({
  selector: 'app-analysis-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    MatProgressBarModule,
    MatChipsModule,
    MatIconModule,
    MatTabsModule,
    MatFormFieldModule,
    MatInputModule,
    FormsModule
  ],
  templateUrl: './analysis-dialog.html',
  styleUrls: ['./analysis-dialog.scss'],
})
export class AnalysisDialogComponent {
  private jobService = inject(JobService);

  // Computed signal to track the specific job from the store
  public trackedJob = computed(() => 
    this.jobService.jobs().find(j => j.id === this.data.jobId)
  );

  // Computed analysis status
  public analysisStatus = computed(() => this.trackedJob()?.analysisStatus);
  public activeOperation = computed(() => this.trackedJob()?.activeOperation);

  // Computed result parsing
  public analysisData = computed(() => {
    const resultStr = this.trackedJob()?.aiAnalysisResult;
    if (resultStr) {
      try {
        return JSON.parse(resultStr) as AiAnalysisResult;
      } catch (e) {
        console.error("Failed to parse analysis result", e);
        return null;
      }
    }
    return null;
  });

  // Computed generated documents
  public generatedResume = computed(() => this.trackedJob()?.generatedResume);
  public generatedCoverLetter = computed(() => this.trackedJob()?.generatedCoverLetter);
  public resumeText = computed(() => this.trackedJob()?.resumeText);

  public errorMessage = signal<string | null>(null);

  private signalRService = inject(SignalRService); // Explicitly inject to listen to errors

  constructor(
    public dialogRef: MatDialogRef<AnalysisDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { jobId: number; jobTitle: string }
  ) {
      // React to specific Job Errors from SignalR
      effect(() => {
          const errors = this.signalRService.jobErrors();
          if (errors.has(this.data.jobId)) {
               this.errorMessage.set(errors.get(this.data.jobId) || 'Unknown error');
          }
      }, { allowSignalWrites: true });
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.startAnalysis(file);
    }
  }

  startAnalysis(file: File) {
    this.errorMessage.set(null);
    this.jobService.analyzeJob(this.data.jobId, file).subscribe({
      next: () => {},
      error: (err) => {
        console.error('AI Analysis Failed:', err);
        this.handleError(err);
      },
    });
  }

  generateResume() {
      this.errorMessage.set(null);
      this.jobService.generateResume(this.data.jobId).subscribe({
          next: () => {},
          error: (err) => this.handleError(err)
      });
  }

  generateCoverLetter() {
      this.errorMessage.set(null);
      this.jobService.generateCoverLetter(this.data.jobId).subscribe({
          next: () => {},
          error: (err) => this.handleError(err)
      });
  }

  private handleError(err: any) {
     if (err.status === 429) {
          this.errorMessage.set('⏳ Rate limit exceeded. Try again in 1 minute.');
        } else {
          this.errorMessage.set('❌ Operation failed. Please try again.');
        }
  }

  close() {
    this.dialogRef.close();
  }

  getScoreColor(score: number): string {
    if (score >= 80) return 'primary';
    if (score >= 50) return 'accent';
    return 'warn';
  }
}
