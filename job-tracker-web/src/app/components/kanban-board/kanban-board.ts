// src/app/components/kanban-board/kanban-board.ts

import { Component, inject, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common'; // Needed for *ngFor, *ngIf
// Import DragDropModule from Angular CDK for drag and drop functionality
import { CdkDragDrop, DragDropModule, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { MatCardModule } from '@angular/material/card'; // For job cards
import { MatButtonModule } from '@angular/material/button';
// Import our service and model
import { JobService, JobApplication } from '../../services/job'; 

@Component({
  selector: 'app-kanban-board',
  // Make sure to add all necessary modules here, especially DragDropModule
  standalone: true,
  imports: [
    DatePipe, 
    DragDropModule, 
    MatCardModule, 
    MatButtonModule
  ],
  templateUrl: './kanban-board.html',
  styleUrls: ['./kanban-board.scss']
})
export class KanbanBoardComponent implements OnInit {
  // Use inject to get the JobService instance
  private jobService = inject(JobService);

  // Define the columns for the Kanban board
  // This is the array that holds our JobApplication objects
  public applied: JobApplication[] = [];
  public interviewing: JobApplication[] = [];
  public offer: JobApplication[] = [];
  public rejected: JobApplication[] = [];

  // A list of the column IDs, needed for the Drag and Drop Module
  public columnIds: string[] = ['applied-list', 'interviewing-list', 'offer-list', 'rejected-list'];

  // This lifecycle hook runs when the component is initialized
  ngOnInit(): void {
    // 1. Fetch data from the backend
    this.loadJobs();
  }

  // Method to fetch data and categorize it into the columns
  loadJobs(): void {
    this.jobService.getJobs().subscribe({
      next: (jobs) => {
        // Clear existing arrays before categorization
        this.applied = [];
        this.interviewing = [];
        this.offer = [];
        this.rejected = [];
        
        // Categorize jobs based on the 'status' property
        jobs.forEach(job => {
          switch (job.status) {
            case 'Applied':
              this.applied.push(job);
              break;
            case 'Interviewing':
              this.interviewing.push(job);
              break;
            case 'Offer':
              this.offer.push(job);
              break;
            case 'Rejected':
              this.rejected.push(job);
              break;
            default:
              this.applied.push(job); // Default to 'Applied' if status is unknown
          }
        });
      },
      error: (err) => {
        console.error('Error fetching job applications:', err);
        // In a real app, we would show a user-friendly error message here
      }
    });
  }

  /**
   * Handles the drop event when a job card is moved.
   */
  drop(event: CdkDragDrop<JobApplication[]>) {
    // If the card is dropped back into the same list
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } 
    // If the card is dropped into a different list
    else {
      transferArrayItem(
        event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex,
      );
      
      // CRITICAL STEP 1: Update the status of the moved item in the model
      const movedJob = event.container.data[event.currentIndex];
      
      // Determine the new status based on the target container ID
      let newStatus: string;
      switch (event.container.id) {
        case 'interviewing-list':
          newStatus = 'Interviewing';
          break;
        case 'offer-list':
          newStatus = 'Offer';
          break;
        case 'rejected-list':
          newStatus = 'Rejected';
          break;
        case 'applied-list':
        default:
          newStatus = 'Applied';
          break;
      }

      // CRITICAL STEP 2: Update the model and call the backend
      movedJob.status = newStatus;
      console.log(`Job ID ${movedJob.id} moved to status: ${newStatus}`);

      // Call the service to update the job status in the database
      this.jobService.updateJob(movedJob).subscribe({
        next: () => console.log('Backend status update successful.'),
        error: (err) => {
          console.error('Failed to update job status on backend:', err);
          // TODO: Implement error handling (e.g., move the card back on failure)
        }
      });
    }
  }
}