import {
  CdkDragDrop,
  DragDropModule,
  moveItemInArray,
  transferArrayItem,
} from '@angular/cdk/drag-drop';
import { DatePipe, NgTemplateOutlet } from '@angular/common';
import { Component, OnInit, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';

import { JobApplication } from '../../models/job-application.model';
import { JobService } from '../../services/job.service';
import { AnalysisDialogComponent } from '../analysis-dialog/analysis-dialog';
import { JobDialogComponent } from '../job-dialog/job-dialog';

@Component({
  selector: 'app-kanban-board',
  standalone: true,
  imports: [
    NgTemplateOutlet,
    DatePipe, // For {{ date | date }}
    DragDropModule, // For Drag and Drop
    MatCardModule, // For Material Cards
    MatButtonModule,
    MatIconModule, // Add this for the button icon
    MatMenuModule, // Add this for the menu
  ],
  templateUrl: './kanban-board.html',
  styleUrls: ['./kanban-board.scss'],
})
export class KanbanBoardComponent implements OnInit {
  private jobService = inject(JobService);
  private dialog = inject(MatDialog);

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

  // Access the search query
  public searchQuery = this.jobService.searchQuery;

  // Filter logic now includes text search
  public appliedJobs = computed(() => this.filterJobs(this.statuses.APPLIED));
  public interviewingJobs = computed(() => this.filterJobs(this.statuses.INTERVIEWING));
  public offerJobs = computed(() => this.filterJobs(this.statuses.OFFER));
  public rejectedJobs = computed(() => this.filterJobs(this.statuses.REJECTED));

  // Centralized filter function
  private filterJobs(status: string): JobApplication[] {
    const query = this.searchQuery().toLowerCase().trim();

    return this.jobs().filter((job) => {
      // 1. Filter by Status
      const matchesStatus = job.status === status;

      // 2. Filter by Search Query (Title or Company)
      const matchesQuery =
        !query ||
        job.jobTitle.toLowerCase().includes(query) ||
        job.companyName.toLowerCase().includes(query);

      return matchesStatus && matchesQuery;
    });
  }

  ngOnInit(): void {
    this.jobService.getAllJobs();
  }

  getStatusListId(status: string): string {
    switch (status) {
      case this.statuses.APPLIED:
        return 'applied-list';
      case this.statuses.INTERVIEWING:
        return 'interviewing-list';
      case this.statuses.OFFER:
        return 'offer-list';
      case this.statuses.REJECTED:
        return 'rejected-list';
      default:
        return 'applied-list';
    }
  }

  /**
   * Method to open dialog in Edit Mode
   */
  editJob(job: JobApplication): void {
    this.dialog.open(JobDialogComponent, {
      width: '600px',
      disableClose: true,
      data: { job: job }, // Pass the job object to indicate Edit Mode
    });
  }

  /**
   * Deletes a specific job application after prompting the user for confirmation.
   */
  deleteJob(jobId: number): void {
    if (confirm('Delete this job?')) {
      this.jobService.deleteJob(jobId).subscribe();
    }
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

  /**
   * Opens the AI Analysis Dialog.
   * Triggered by the "Analyze AI" button in the HTML.
   */
  openAnalysis(jobId: number | undefined): void {
    if (!jobId) return;

    // We need to find the job title to display in the dialog header
    // We can look it up in the current signal list
    const job = this.jobs().find((j) => j.id === jobId);
    const title = job ? job.jobTitle : 'Job Application';

    this.dialog.open(AnalysisDialogComponent, {
      width: '500px',
      data: {
        jobId: jobId,
        jobTitle: title,
      },
    });
  }
}
