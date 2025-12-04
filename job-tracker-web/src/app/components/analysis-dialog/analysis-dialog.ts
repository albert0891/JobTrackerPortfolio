import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';

// Define the shape of the data we expect from the AI
export interface AiAnalysisData {
  matchScore: number;
  strengths: string;
  keywordsToIntegrate: string[];
}

@Component({
  selector: 'app-analysis-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatListModule, MatIconModule],
  templateUrl: './analysis-dialog.html',
  styleUrls: ['./analysis-dialog.scss'],
})
export class AnalysisDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<AnalysisDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: AiAnalysisData
  ) {}

  getScoreClass(score: number): string {
    if (score >= 80) return 'high-score';
    if (score >= 50) return 'med-score';
    return 'low-score';
  }
}
