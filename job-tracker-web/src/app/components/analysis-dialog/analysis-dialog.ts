import { Component, Inject, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar'; // For loading state
import { MatChipsModule } from '@angular/material/chips'; // For keywords
import { MatIconModule } from '@angular/material/icon';

import { JobService } from '../../services/job.service';
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
  ],
  templateUrl: './analysis-dialog.html',
  styleUrls: ['./analysis-dialog.scss'],
})
export class AnalysisDialogComponent {
  private jobService = inject(JobService);

  // Signals to manage UI state
  public isLoading = signal<boolean>(false);
  public analysisData = signal<AiAnalysisResult | null>(null);
  public errorMessage = signal<string | null>(null);

  constructor(
    public dialogRef: MatDialogRef<AnalysisDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { jobId: number; jobTitle: string }
  ) {}

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.startAnalysis(file);
    }
  }

  startAnalysis(file: File) {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.analysisData.set(null);

    // Pass the file to the service
    this.jobService.analyzeJob(this.data.jobId, file).subscribe({
      next: (result) => {
        this.analysisData.set(result);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('AI Analysis Failed:', err);
        this.errorMessage.set('Analysis failed. Please try again.');
        this.isLoading.set(false);
      },
    });
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
