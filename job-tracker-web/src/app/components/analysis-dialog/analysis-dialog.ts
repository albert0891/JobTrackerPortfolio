import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';

// Define the shape of the data we expect from the AI
export interface AiAnalysisData {
  matchScore: number;
  strengths: string;
  keywordsToIntegrate: string[];
}

@Component({
  selector: 'app-analysis-dialog',
  standalone: true,
  imports: [
    CommonModule, 
    MatDialogModule, 
    MatButtonModule, 
    MatListModule,
    MatIconModule
  ],
  template: `
    <h2 mat-dialog-title>AI Resume Analysis</h2>
    <mat-dialog-content>
      <div class="score-section">
        <h3>Match Score</h3>
        <div class="score-circle" [ngClass]="getScoreClass(data.matchScore)">
          {{ data.matchScore }}%
        </div>
      </div>

      <div class="analysis-section">
        <h3>Strengths</h3>
        <p>{{ data.strengths }}</p>

        <h3>Missing Keywords</h3>
        <mat-list>
          @for (keyword of data.keywordsToIntegrate; track keyword) {
            <mat-list-item>
              <mat-icon matListItemIcon>warning</mat-icon>
              <div matListItemTitle>{{ keyword }}</div>
            </mat-list-item>
          }
        </mat-list>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Close</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .score-section { text-align: center; margin-bottom: 20px; }
    .score-circle {
      width: 80px; height: 80px; border-radius: 50%;
      display: flex; align-items: center; justify-content: center;
      margin: 0 auto; font-size: 24px; font-weight: bold; color: white;
    }
    .high-score { background-color: #4caf50; } /* Green */
    .med-score { background-color: #ff9800; }  /* Orange */
    .low-score { background-color: #f44336; }  /* Red */
    h3 { color: #555; border-bottom: 1px solid #eee; padding-bottom: 5px; }
  `]
})
export class AnalysisDialogComponent {
  // Inject the data passed into the dialog
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