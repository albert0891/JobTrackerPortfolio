import { inject, Injectable, signal, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JobApplication } from '../models/job-application.model';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs';
import { AiAnalysisResult } from '../models/job-application.model';
import { SignalRService } from './signalr.service';

@Injectable({
  providedIn: 'root',
})
export class JobService {
  private http = inject(HttpClient);
  private signalRService = inject(SignalRService); // Inject SignalR Service

  private apiUrl = `${environment.apiUrl}/JobApplications`;

  private jobsSignal = signal<JobApplication[]>([]);
  public readonly jobs = this.jobsSignal.asReadonly();
  public searchQuery = signal<string>('');

  constructor() {
    // React to Status Updates from SignalR
    effect(() => {
      const updates = this.signalRService.jobStatusUpdates();
      if (updates.size === 0) return;

      this.jobsSignal.update(jobs => 
        jobs.map(job => {
          if (job.id && updates.has(job.id)) {
            const update = updates.get(job.id);
            if (update) {
                return { 
                    ...job, 
                    analysisStatus: update.status,
                    activeOperation: update.operation 
                };
            }
          }
          return job;
        })
      );
    }, { allowSignalWrites: true });

    // React to Analysis Results from SignalR
    effect(() => {
      const results = this.signalRService.analysisResults();
      if (results.size === 0) return;

      this.jobsSignal.update(jobs => 
        jobs.map(job => {
          if (job.id && results.has(job.id)) {
            const resultObj = results.get(job.id);
            // We need to store it as string to match the model expectation for persistence (if we were saving purely frontend, but here we just update for display)
            // The DB update happens in backend.
            return { 
              ...job, 
              aiAnalysisResult: JSON.stringify(resultObj), 
              analysisStatus: 'Done' 
            };
          }
          return job;
        })
      );
    }, { allowSignalWrites: true });
    // React to Resume Generated
    effect(() => {
        const resumes = this.signalRService.resumeGenerated();
        if (resumes.size === 0) return;

        this.jobsSignal.update(jobs => 
            jobs.map(job => {
                if (job.id && resumes.has(job.id)) {
                    return { ...job, generatedResume: resumes.get(job.id), analysisStatus: 'Done' };
                }
                return job;
            })
        );
    }, { allowSignalWrites: true });

    // React to Cover Letter Generated
    effect(() => {
        const letters = this.signalRService.coverLetterGenerated();
        if (letters.size === 0) return;

        this.jobsSignal.update(jobs => 
            jobs.map(job => {
                if (job.id && letters.has(job.id)) {
                    return { ...job, generatedCoverLetter: letters.get(job.id), analysisStatus: 'Done' };
                }
                return job;
            })
        );
    }, { allowSignalWrites: true });
  }

  // --- METHODS ---
  
  getAllJobs() {
    this.http.get<JobApplication[]>(this.apiUrl).subscribe({
      next: (data) => {
        console.log('Jobs fetched successfully:', data);
        this.jobsSignal.set(data);
      },
      error: (err) => {
        console.error('Error fetching jobs:', err);
      },
    });
  }

  addJob(job: JobApplication) {
    return this.http.post<JobApplication>(this.apiUrl, job).pipe(
      tap(() => {
        this.getAllJobs();
      })
    );
  }

  updateJobStatus(jobId: number, newStatus: string) {
    const url = `${this.apiUrl}/status/${jobId}`;
    this.http.put(url, { status: newStatus }).subscribe({
      next: () => {
        console.log(`Status updated to ${newStatus} for Job ID ${jobId}`);
        this.jobsSignal.update((currentJobs) =>
          currentJobs.map((job) => {
            if (job.id === jobId) {
              return { ...job, status: newStatus };
            }
            return job;
          })
        );
      },
      error: (err) => {
        console.error('Error updating status:', err);
      },
    });
  }

  updateJob(job: JobApplication) {
    const url = `${this.apiUrl}/${job.id}`;
    return this.http.put(url, job).pipe(
      tap(() => {
        this.jobsSignal.update((jobs) => jobs.map((j) => (j.id === job.id ? job : j)));
      })
    );
  }

  deleteJob(jobId: number) {
    const url = `${this.apiUrl}/${jobId}`;
    return this.http.delete(url).pipe(
      tap(() => {
        this.jobsSignal.update((jobs) => jobs.filter((j) => j.id !== jobId));
      })
    );
  }

   /**
   * Calls the AI endpoint with a resume file.
   * Now returns 202 Accepted immediately.
   */
  analyzeJob(jobId: number, resumeFile: File) {
    const url = `${environment.apiUrl}/Ai/analyze/${jobId}`;

    const formData = new FormData();
    formData.append('resume', resumeFile);

    // Optimistic Update
    this.setAnalysisStatus(jobId, 'Queued');

    return this.http.post(url, formData);
  }

  /**
   * Triggers Resume Generation
   */
  generateResume(jobId: number) {
      const url = `${environment.apiUrl}/Ai/generate-resume/${jobId}`;
      this.setAnalysisStatus(jobId, 'Queued', 'Resume');
      return this.http.post(url, {});
  }

  /**
   * Triggers Cover Letter Generation
   */
  generateCoverLetter(jobId: number) {
      const url = `${environment.apiUrl}/Ai/generate-cover-letter/${jobId}`;
      this.setAnalysisStatus(jobId, 'Queued', 'CoverLetter');
      return this.http.post(url, {});
  }

  private setAnalysisStatus(jobId: number, status: string, operation?: string) {
    this.jobsSignal.update(jobs => 
        jobs.map(j => j.id === jobId ? { ...j, analysisStatus: status, activeOperation: operation } : j)
    );
  }
}
