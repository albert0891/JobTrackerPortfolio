import { Component, OnInit, inject } from '@angular/core';
// Import DatePipe to format dates in HTML
import { DatePipe } from '@angular/common';
import {
  CdkDragDrop,
  DragDropModule,
  moveItemInArray,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

// Import our new Model and Service
import { JobApplication } from '../../models/job-application.model';
import { JobService } from '../../services/job.service';

@Component({
  selector: 'app-kanban-board',
  standalone: true,
  imports: [
    DatePipe, // For {{ date | date }}
    DragDropModule, // For Drag and Drop
    MatCardModule, // For Material Cards
    MatButtonModule,
  ],
  templateUrl: './kanban-board.html',
  styleUrls: ['./kanban-board.scss'],
})
export class KanbanBoardComponent implements OnInit {
  // Inject the JobService
  private jobService = inject(JobService);

  // Expose the Service's Signal directly to the template.
  // Note: 'jobs' is a Signal, so it is called like a function: this.jobs()
  public jobs = this.jobService.jobs;

  // Define column IDs for drag-and-drop connection
  public columnIds = ['applied-list', 'interviewing-list', 'offer-list', 'rejected-list'];

  // Constants for status strings to avoid typos
  public readonly statuses = {
    APPLIED: 'Applied',
    INTERVIEWING: 'Interviewing',
    OFFER: 'Offer',
    REJECTED: 'Rejected',
  };

  ngOnInit(): void {
    // Initial fetch of data when component loads
    this.jobService.getAllJobs();
  }

  /**
   * Helper function to filter the Signal's list by status.
   * This is used in the HTML @for loop.
   */
  getJobsByStatus(status: string): JobApplication[] {
    // Access the signal value using parenthesis ()
    return this.jobs().filter((job) => job.status === status);
  }

  /**
   * Handles the drop event from the Angular CDK DragDropModule.
   */
  drop(event: CdkDragDrop<JobApplication[]>) {
    // Case 1: Reordering within the same column
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    }
    // Case 2: Moving to a different column
    else {
      // 1. Visually move the item immediately
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex
      );

      // 2. Get the Job object that was moved
      const movedJob = event.container.data[event.currentIndex];

      // 3. Determine new status based on the DOM ID of the column
      let newStatus = this.statuses.APPLIED; // Default

      switch (event.container.id) {
        case 'interviewing-list':
          newStatus = this.statuses.INTERVIEWING;
          break;
        case 'offer-list':
          newStatus = this.statuses.OFFER;
          break;
        case 'rejected-list':
          newStatus = this.statuses.REJECTED;
          break;
        case 'applied-list':
          newStatus = this.statuses.APPLIED;
          break;
      }

      // 4. Call Service to update Backend
      // We check for 'id' existence to satisfy TypeScript strict mode
      if (movedJob.id) {
        this.jobService.updateJobStatus(movedJob.id, newStatus);
      }
    }
  }
}
