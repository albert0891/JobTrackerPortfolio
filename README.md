# 🧲 OfferMagnet (AI-Powered Job Tracker)

**OfferMagnet** is a modern, full-stack intelligent job application tracker designed to streamline your job search process. It combines a drag-and-drop Kanban board with Google Gemini AI to analyze job descriptions against your resume, providing actionable insights to increase your interview chances.

## 🌟 Key Features

- **📋 Smart Kanban Board:** Visualize your job search pipeline with an intuitive drag-and-drop interface across four stages: _Applied_, _Interviewing_, _Offer_, and _Rejected_.
- **🧠 AI-Powered Analysis:** Seamlessly integrated with **Google Gemini 1.5 Flash**. Click "Analyze" on any job card to get:
  - A match score (0-100).
  - Key strengths analysis.
  - Missing keywords suggestions to optimize your resume.
- **🖱️ Drag & Drop Interface:** Built with Angular CDK for smooth, responsive interactions.
- **💾 Robust Persistence:** Data is securely stored using a Dockerized **PostgreSQL** database managed via **Entity Framework Core**.
- **🎨 Modern UI:** A clean, glassmorphism-inspired design using Angular Material.

## 🛠️ Technology Stack

### Frontend (Web)

- **Framework:** Angular 17+ (Standalone Components, Signals)
- **UI Library:** Angular Material & Angular CDK
- **Styling:** SCSS with modern CSS variables
- **State Management:** Angular Signals (Native reactive state)

### Backend (API)

- **Framework:** .NET 9 Web API
- **Language:** C#
- **Database:** PostgreSQL 15 (Docker Container)
- **ORM:** Entity Framework Core
- **AI Integration:** Google Gemini API (via `Mscc.GenerativeAI`)
- **Documentation:** Swagger / OpenAPI

## ⚙️ Prerequisites

Ensure you have the following installed on your local machine:

1.  **Node.js** (LTS version)
2.  **Angular CLI**
3.  **.NET 9 SDK**
4.  **Docker Desktop** (Must be running for the database)
5.  **Google Gemini API Key** (Obtain from [Google AI Studio](https://aistudio.google.com/))

## 🚀 Installation & Setup Guide

### 1. Clone the Repository

```bash
git clone [https://github.com/albert0891/OfferMagnet.git](https://github.com/albert0891/OfferMagnet.git)
cd OfferMagnet
```

### 2. Backend Setup (API & Database)

**Step A: Secure your AI API Key**
We use .NET User Secrets to keep your API key secure.

```bash
cd JobTracker.Api

# Initialize User Secrets
dotnet user-secrets init

# Set your Google Gemini API Key
dotnet user-secrets set "Gemini:ApiKey" "YOUR_ACTUAL_GOOGLE_API_KEY_HERE"
```

**Step B: Start the Database**
Ensure Docker Desktop is running, then spin up the PostgreSQL container:

```bash
# Still in the JobTracker.Api folder
docker-compose up -d
```

**Step C: Run the API**
Start the backend server. It will automatically apply database migrations and seed initial data.

```bash
dotnet watch run
```

_The API will typically start at `http://localhost:5023` or `https://localhost:7xxx`. Check your terminal output._

### 3. Frontend Setup (Web App)

Open a new terminal window and navigate to the frontend directory:

```bash
cd job-tracker-web

# Install dependencies
npm install

# Start the development server
ng serve -o
```

_The application will automatically open at `http://localhost:4200`._

## 📌 How to Use

1.  **Dashboard:** Navigate to `http://localhost:4200` to view your Kanban board.
2.  **Add a Job:** Click the **"New Job"** button in the top toolbar. Fill in the details (Title, Company, Status, JD) and save.
3.  **Manage Status:** Drag and drop cards between columns (e.g., from _Applied_ to _Interviewing_) to update their status instantly.
4.  **AI Analysis:**
    - Locate a job card in the **Applied** column.
    - Click the **"Analyze"** button.
    - Wait for Gemini AI to evaluate the job description and provide a match score and keyword suggestions.

## 📂 Project Structure

```text
OfferMagnet/
├── JobTracker.Api/           # .NET 9 Web API
│   ├── Controllers/          # API Endpoints (JobApplications, AI)
│   ├── Data/                 # EF Core Context & Migrations
│   ├── Models/               # C# Entities & DTOs
│   ├── Services/             # Business Logic (AI, Job management)
│   └── docker-compose.yml    # PostgreSQL Configuration
│
└── job-tracker-web/          # Angular 17+ Frontend
    ├── src/app/
    │   ├── components/       # Standalone Components (Kanban, Dialogs)
    │   ├── models/           # TypeScript Interfaces
    │   ├── services/         # HTTP Services & Signals
    │   └── app.routes.ts     # Routing Configuration
    └── styles.scss           # Global Styles & Theming
```
