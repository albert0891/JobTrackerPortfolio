export interface JobApplication {
  // Optional because it's assigned by the database on creation
  id?: number;

  jobTitle: string;
  companyName: string;
  dateApplied: Date;
  status: string; // e.g., "Applied", "Interviewing", "Offer", "Rejected"
  jobDescription: string;

  // We include these fields now to match the Backend DB structure,
  // even if we aren't using the AI logic yet.
  aiAnalysisResult?: string;
  generatedCoverLetter?: string;
  generatedResume?: string;
  resumeText?: string;

  // Transient field for UI state (Queued, Processing, Done, Failed)
  analysisStatus?: string;
  // Transient: 'Analyze', 'Resume', 'CoverLetter'
  activeOperation?: string;
}

export interface AiAnalysisResult {
  matchScore: number;
  strengths: string;
  keywordsToIntegrate: string[];
}
