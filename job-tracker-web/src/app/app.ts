import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { RouterOutlet } from '@angular/router';

import { AddJobDialogComponent } from './components/add-job-dialog/add-job-dialog';

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
    this.dialog.open(AddJobDialogComponent, {
      width: '600px',
      disableClose: true, // Force user to click Cancel or Save
    });
  }
}
