# OfferMagnet 🧲

**OfferMagnet** is a modern, AI-powered job application tracker designed to optimize the job hunting workflow. It combines a visual **Kanban** interface with **Generative AI** to analyze job descriptions, **tailor resumes**, and **draft cover letters** in real-time.

<div align="center">
  <img src="docs/dashboard-preview.png" alt="Dashboard Screenshot" width="70%" />
  <img src="docs/ai-analysis-result-preview.png" alt="AI Analysis Result Screenshot" width="25%" />
</div>

## 🚀 Features

### 1. Visual Kanban Board
- **Drag-and-Drop Interface**: Built with Angular CDK for intuitive status management (Applied, Interviewing, Offer, Rejected).
- **Interactive UI**: Custom Material Design cards with independent column scrolling.
- **Job Validation**: Checks for missing descriptions or company names before analysis.

### 2. AI-Powered Analysis & Documents
- **Gemini Integration**: Uses Google's Gemini 1.5 Flash/2.5 for high-speed analysis.
- **Smart Scoring**: Instantly scores your resume against the Job Description (JD) and highlights missing keywords.
- **Tailored Resumes**: Automatically rewrites your resume content to match the specific JD using GenAI.
- **Cover Letter Generator**: Drafts compelling, structured (Hook-Meat-Fit-CTA) cover letters in seconds.
- **Iterative Workflow**: Upload a new resume version and re-analyze instantly to see your score improve.
- **PdfPig Parsing**: robustly extracts text from PDF resumes.

### 3. Real-Time Async Architecture
- **Non-blocking UI**: Analysis requests are queued instantly (`202 Accepted`), keeping the UI responsive.
- **SignalR**: Real-time WebSocket connection pushes progress updates ("Thinking...", "Writing Cover Letter...") and final results to the client.
- **Background Processing**: Heavy AI tasks are handled by `.NET BackgroundService` workers using specialized Channels.

### 4. Modern Tech Stack
- **Frontend**: Angular 18+ (Standalone Components), Signals, Angular Material, RxJS.
- **Backend**: .NET 9 Web API, Entity Framework Core, SignalR Hubs.
- **Cloud-Native**: Serverless PostgreSQL (Neon), Rate Limiting, Docker-ready.

---

## 🛠️ Tech Stack

### Frontend
- **Framework**: Angular 18+ (Standalone Components)
- **Language**: TypeScript
- **State Management**: Angular Signals & RxJS
- **Real-time**: `@microsoft/signalr`
- **UI Library**: Angular Material, CDK
- **Styling**: SCSS (Global & Scoped)

### Backend
- **Framework**: .NET 9 Web API
- **Language**: C#
- **Real-time**: ASP.NET Core SignalR
- **AI Integration**: Google Gemini API
- **Background Jobs**: `System.Threading.Channels`, `BackgroundService`
- **ORM**: Entity Framework Core (SQLite/PostgreSQL)

---

## ⚙️ Getting Started

### Prerequisites
- Node.js (v18+)
- .NET 9 SDK
- Docker Desktop (optional, for PostgreSQL)

### 1. Backend Setup (.NET API)

Navigate to the API directory and configure your secrets.

```bash
cd JobTracker.Api

# 1. Set your Gemini API Key (Securely)
dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "YOUR_GOOGLE_GEMINI_KEY"

# 2. Apply Database Migrations
dotnet ef database update

# 3. Run the Server
dotnet watch run
```
_The API will start at `http://localhost:5023`._

### 2. Frontend Setup (Angular)

Navigate to the Web directory.

```bash
cd job-tracker-web

# 1. Install Dependencies
npm install

# 2. Run the Application
ng serve
```
_Open your browser and navigate to `http://localhost:4200`._

---

## 📂 Project Structure

```
OfferMagnet/
├── docs/                     # Documentation images
│
├── JobTracker.Api/           # .NET 9 Web API
│   ├── Controllers/          # API Endpoints (AiController, JobAppsController)
│   ├── Hubs/                 # SignalR Hubs (AnalysisHub)
│   ├── Services/             # Business Logic (AiService, BackgroundAnalysisService)
│   ├── Models/               # EF Core Entities
│   └── Program.cs            # DI & App Configuration
│
└── job-tracker-web/          # Angular Frontend
    └── src/app/
        ├── components/       # Standalone Components (AnalysisDialog, Kanban)
        ├── models/           # TypeScript Interfaces
        ├── services/         # JobService (Signals), SignalRService
        └── app.routes.ts     # Routes
```

## 🔐 Configuration

**Rate Limiting**: The API uses a fixed window limiter (5 requests/minute) to prevent abusing the Gemini API. Exceeding this will return `429 Too Many Requests`.

### AppSettings (Backend)

The database connection string is located in `appsettings.json`. For the API Key, it is recommended to use **User Secrets** during development to avoid committing credentials to Git.

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5433;Database=jobtrackerdb;Username=myuser;Password=mypassword"
}
```

### Demo Mode (Optional)

This project supports a "self-cleaning" mode for public demos.

```json
"DemoMode": {
  "Enabled": true,                 // CAUTION: Deletes data every X minutes
  "CleanupIntervalMinutes": 60,    // Reset interval
  "SeedData": true                 // Reseed with sample data
}
```

> **Warning**: Never enable `DemoMode:Enabled` in a production environment with real user data.

---

## 📦 Deployment & CI/CD

This project is configured for seamless cloud deployment.

### Database (Neon)

The database is hosted on **Neon**. We use a hybrid migration strategy:

- **Development**: Manually apply migrations or use `dotnet ef database update`.
- **Production**: The API includes a runtime migration check in `Program.cs`. When Render deploys a new build, the app automatically executes `context.Database.Migrate()` to update the schema without downtime.

### API (Render)

- **Build Command**: `dotnet build`
- **Start Command**: `dotnet run`
- **Environment Variables**:
  - `Gemini__ApiKey`: Set in Render Dashboard.
  - `ConnectionStrings__DefaultConnection`: The Neon Pooled connection string.
