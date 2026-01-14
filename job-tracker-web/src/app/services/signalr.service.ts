import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { AiAnalysisResult } from '../models/job-application.model';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: signalR.HubConnection | undefined;

  // Signal to expose updates to components
  // Key: JobId, Value: { status, operation }
  public jobStatusUpdates = signal<Map<number, { status: string, operation: string }>>(new Map());

  // Signal to hold the latest analysis result
  // Key: JobId, Value: Result
  public analysisResults = signal<Map<number, AiAnalysisResult>>(new Map());

  // Signal to hold the latest resume generated
  public resumeGenerated = signal<Map<number, string>>(new Map());

  // Signal to hold the latest cover letter generated
  public coverLetterGenerated = signal<Map<number, string>>(new Map());

  // Signal for errors
  public jobErrors = signal<Map<number, string>>(new Map());

  constructor() {
    this.startConnection();
  }

  private startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl.replace('/api', '')}/analysisHub`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connection started'))
      .catch(err => console.error('Error while starting connection: ' + err));

    this.registerHandlers();
  }

  private registerHandlers() {
    if (!this.hubConnection) return;

    this.hubConnection.on('JobStatusUpdate', (jobId: number, status: string, operation: string) => {
      console.log(`SignalR: Job ${jobId} status: ${status} [${operation}]`);
      this.jobStatusUpdates.update(map => {
        const newMap = new Map(map);
        newMap.set(jobId, { status, operation });
        return newMap;
      });
    });
    
    // ... (other handlers)

    this.hubConnection.on('JobError', (jobId: number, message: string) => {
        console.error(`SignalR: Job ${jobId} error: ${message}`);
        this.jobErrors.update(map => {
            const newMap = new Map(map);
            newMap.set(jobId, message);
            return newMap;
        });
    });
    
    // ... (rest of handlers)

    this.hubConnection.on('AnalysisComplete', (jobId: number, result: AiAnalysisResult) => {
      console.log(`SignalR: Job ${jobId} analysis complete`, result);
      
      this.analysisResults.update(map => {
        const newMap = new Map(map);
        newMap.set(jobId, result);
        return newMap;
      });
    });

    this.hubConnection.on('ResumeGenerated', (jobId: number, result: string) => {
        console.log(`SignalR: Job ${jobId} resume generated`);
        this.resumeGenerated.update(map => {
            const newMap = new Map(map);
            newMap.set(jobId, result);
            return newMap;
        });
    });

    this.hubConnection.on('CoverLetterGenerated', (jobId: number, result: string) => {
        console.log(`SignalR: Job ${jobId} cover letter generated`);
        this.coverLetterGenerated.update(map => {
            const newMap = new Map(map);
            newMap.set(jobId, result);
            return newMap;
        });
    });
  }
}
