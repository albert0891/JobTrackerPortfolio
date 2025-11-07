// src/app/services/job.ts (Your Angular Service File)

import { inject, Injectable } from '@angular/core';
// IMPORTANT: We use inject() for modern Angular services
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// Interface matching the C# Model (critical for Type Safety)
export interface JobApplication {
  id?: number; 
  jobTitle: string;
  companyName: string;
  dateApplied: Date;
  status: string;
  jobDescription: string;
  aiAnalysisResult?: string; 
  generatedCoverLetter?: string; 
}

@Injectable({
  providedIn: 'root'
})
export class JobService {
  // Use the modern 'inject' function to get the HttpClient instance
  private http = inject(HttpClient); 
  
  // !!! CRITICAL: UPDATE THIS URL TO YOUR ACTUAL HTTPS PORT (e.g., 7088, 7251) !!!
  private apiUrl = 'http://localhost:5023/api/JobApplications'; 

  constructor() { } // The constructor is now empty thanks to inject()

  /**
   * Retrieves all job applications from the backend.
   * Corresponds to: GET /api/JobApplications
   */
  getJobs(): Observable<JobApplication[]> {
    console.log(`Calling GET: ${this.apiUrl}`);
    return this.http.get<JobApplication[]>(this.apiUrl);
  }

  /**
   * Creates a new job application entry.
   * Corresponds to: POST /api/JobApplications
   */
  addJob(job: JobApplication): Observable<JobApplication> {
    return this.http.post<JobApplication>(this.apiUrl, job);
  }

  /**
   * Updates an existing job application, typically for status change.
   * Corresponds to: PUT /api/JobApplications/{id}
   */
  updateJob(job: JobApplication): Observable<any> {
    return this.http.put(`${this.apiUrl}/${job.id}`, job);
  }
}