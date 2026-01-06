import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JobApplication } from '../models/job-application.model';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs';
import { AiAnalysisResult } from '../models/job-application.model';

@Injectable({
  providedIn: 'root',
})
export class JobService {
  // Use 'inject' for Dependency Injection (modern Angular style)
  private http = inject(HttpClient);

  // Get the base API URL from the environment config
  private apiUrl = `${environment.apiUrl}/JobApplications`;

  // --- STATE MANAGEMENT (SIGNALS) ---

  // Writable Signal: Holds the current list of job applications.
  // We make it private so components cannot modify it directly.
  private jobsSignal = signal<JobApplication[]>([]);

  // Read-only Signal: Exposed to components.
  // Components will track this signal to update the UI automatically.
  public readonly jobs = this.jobsSignal.asReadonly();

  // Global search state
  public searchQuery = signal<string>('');

  constructor() {}

  /**
   * Fetches all jobs from the backend and updates the signal.
   * HTTP GET: /api/JobApplications
   */
  getAllJobs() {
    this.http.get<JobApplication[]>(this.apiUrl).subscribe({
      next: (data) => {
        console.log('Jobs fetched successfully:', data);
        // Update the signal with the new data.
        this.jobsSignal.set(data);
      },
      error: (err) => {
        console.error('Error fetching jobs:', err);
      },
    });
  }

  /**
   * Adds a new job and refreshes the list.
   * HTTP POST: /api/JobApplications
   */
  addJob(job: JobApplication) {
    return this.http.post<JobApplication>(this.apiUrl, job).pipe(
      tap(() => {
        // After adding, refresh the list to see the new item
        this.getAllJobs();
      })
    );
  }

  /**
   * Updates the status of a job (e.g., drag and drop).
   * HTTP PUT: /api/JobApplications/status/{id}
   */
  updateJobStatus(jobId: number, newStatus: string) {
    const url = `${this.apiUrl}/status/${jobId}`;

    // We send a simple object matching the DTO expected by the backend
    this.http.put(url, { status: newStatus }).subscribe({
      next: () => {
        console.log(`Status updated to ${newStatus} for Job ID ${jobId}`);

        // Optimistic Update: Update the local signal immediately without re-fetching.
        // This makes the UI feel faster.
        this.jobsSignal.update((currentJobs) =>
          currentJobs.map((job) => {
            if (job.id === jobId) {
              // Return a new object with the updated status
              return { ...job, status: newStatus };
            }
            return job;
          })
        );
      },
      error: (err) => {
        console.error('Error updating status:', err);
        // In a real app, you might want to revert the drag-and-drop here
      },
    });
  }

  /**
   * Updates full job details.
   * HTTP PUT: /api/JobApplications/{id}
   */
  updateJob(job: JobApplication) {
    const url = `${this.apiUrl}/${job.id}`;
    return this.http.put(url, job).pipe(
      tap(() => {
        // Update local signal to reflect changes immediately
        this.jobsSignal.update((jobs) => jobs.map((j) => (j.id === job.id ? job : j)));
      })
    );
  }

  /**
   * Deletes a job.
   * HTTP DELETE: /api/JobApplications/{id}
   */
  deleteJob(jobId: number) {
    const url = `${this.apiUrl}/${jobId}`;
    return this.http.delete(url).pipe(
      tap(() => {
        // Remove from local signal
        this.jobsSignal.update((jobs) => jobs.filter((j) => j.id !== jobId));
      })
    );
  }

  /**
   * Calls the AI endpoint with a resume file.
   * HTTP POST (Multipart): /api/Ai/analyze/{jobId}
   */
  analyzeJob(jobId: number, resumeFile: File) {
    const url = `${environment.apiUrl}/Ai/analyze/${jobId}`;

    const formData = new FormData();
    formData.append('resume', resumeFile);

    // Angular HTTP client handles the Content-Type header for FormData automatically
    return this.http.post<AiAnalysisResult>(url, formData);
  }
}
