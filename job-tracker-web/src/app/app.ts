import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterOutlet } from '@angular/router';

import { JobDialogComponent } from './components/job-dialog/job-dialog';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, MatToolbarModule, MatButtonModule, MatIconModule],
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
})
export class App {
  title = 'OfferMagnet';

  private dialog = inject(MatDialog);

  openAddJobDialog() {
    this.dialog.open(JobDialogComponent, {
      width: '600px',
      disableClose: true, // Force user to click Cancel or Save
      data: { job: null }, // Pass null to indicate Add Mode
    });
  }
}
