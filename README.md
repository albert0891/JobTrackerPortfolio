# OfferMagnet 🧲

**OfferMagnet** is a modern, AI-powered job application tracker designed to optimize the job hunting workflow. It combines a visual **Kanban** interface with **Generative AI** to analyze job descriptions against your resume, providing instant feedback and interview prep strategies.

<div align="center">
  <img src="docs/dashboard-preview.png" alt="Dashboard Screenshot" width="70%" />
  <img src="docs/ai-analysis-result-preview.png" alt="AI Analysis Result Screenshot" width="25%" />
</div>

## 🚀 Features

### 1. Visual Kanban Board

- **Drag-and-Drop Interface**: Built with Angular CDK, allowing intuitive status management (Applied, Interviewing, Offer, Rejected).
- **Interactive UI**: Custom Material Design cards with hover effects and quick actions.
- **Scroll Optimization**: Independent column scrolling ensuring headers stay visible while lists remain navigable.

### 2. AI-Powered Analysis

- **Gemini Integration**: Leverages Google's Gemini 1.5/2.5 models to analyze Job Descriptions (JDs).
- **Smart Insights**: Provides instant feedback on key requirements and skill gaps directly from the application card.
- **Resume Parsing**: Utilizes **PdfPig** to extract text from PDF resumes for comparison against JDs.

### 3. Modern Tech Stack

- **Standalone Components**: Fully modular Angular architecture without `NgModule`.
- **Signals**: Utilizes Angular Signals for fine-grained reactivity and state management.
- **Lazy Loading**: Route-based lazy loading for optimal initial load performance.
- **Robust Backend**: .NET 9 Web API using Entity Framework Core for data persistence.
- **Rate Limiting**: Implements server-side rate limiting (Fixed Window: 5 requests/min) to manage AI API usage quotas.

### 4. Cloud-Native Architecture

- **Database**: Serverless PostgreSQL via **Neon**.
- **Backend**: Hosted on **Render** with auto-scaling.
- **Frontend**: Deployed on **Vercel**.

---

## 🛠️ Tech Stack

### Frontend

- **Framework**: Angular 18+ (Standalone Components)
- **Language**: TypeScript
- **UI Library**: Angular Material
- **State Management**: Angular Signals
- **Utilities**: Angular CDK (Drag & Drop), SCSS

### Backend

- **Framework**: .NET 9 Web API
- **Language**: C#
- **ORM**: Entity Framework Core
- **AI Integration**: Google Gemini API (`Mscc.GenerativeAI`)
- **PDF Processing**: UglyToad.PdfPig

### Database & DevOps

- **Production DB**: Neon (Serverless PostgreSQL)
- **Local DB**: Dockerized PostgreSQL
- **Deployment**: Render (Web Service)

---

## ⚙️ Getting Started

### Prerequisites

- Node.js (v18+)
- .NET 9 SDK
- Docker Desktop (for PostgreSQL)

### 1. Database Setup

Ensure your PostgreSQL container is running.

```bash
docker run --name jobtracker-db -e POSTGRES_USER=myuser -e POSTGRES_PASSWORD=mypassword -p 5433:5432 -d postgres
```

### 2. Backend Setup (.NET API)

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

_The API will start at `http://localhost:5023` (or your configured port)._

### 3. Frontend Setup (Angular)

Navigate to the Client directory.

```bash
cd JobTracker.Client

# 1. Install Dependencies
npm install

# 2. Run the Application
ng serve
```

_Open your browser and navigate to `http://localhost:4200`._

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

---

## 📂 Project Structure

```
OfferMagnet/
├── docs/                     # Documentation
│
├── JobTracker.Api/           # .NET 9 Web API
│   ├── Controllers/          # API Endpoints
│   ├── Models/               # EF Core Entities
│   ├── Services/             # Business Logic (AiService.cs, JobService.cs)
│   └── Program.cs            # DI & Configuration
│
└── JobTracker.Client/        # Angular Frontend
    ├── src/app/
    │   ├── components/       # Standalone Components (Kanban, Dialogs)
    │   ├── models/           # TypeScript Interfaces
    │   ├── services/         # HTTP Services
    │   └── app.routes.ts     # Lazy Loaded Routes
```

## 🔐 Configuration

### AppSettings (Backend)

The database connection string is located in `appsettings.json`. For the API Key, it is recommended to use **User Secrets** during development to avoid committing credentials to Git.

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5433;Database=jobtrackerdb;Username=myuser;Password=mypassword"
}
```
