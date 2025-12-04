# 🧲 OfferMagnet (AI-Powered Job Tracker)

OfferMagnet is a modern, full-stack intelligent job application tracker. It helps users manage their job search pipeline using a Kanban board and leverages Google Gemini AI to analyze resume-to-job fit, providing match scores and keyword optimization suggestions.

## 🌟 Features

- **Smart Kanban Board:** Drag-and-drop interface to manage applications across stages (Applied, Interviewing, Offer, Rejected).
- **AI Analysis (Gemini):** Integrates Google's Gemini 1.5 Flash model to analyze job descriptions against your resume, extracting key missing skills and providing a match score.
- **Data Persistence:** Uses a Dockerized PostgreSQL database with Entity Framework Core for reliable data management.
- **Modern Architecture:** Built with .NET 9 Web API (Clean Architecture) and Angular 17+ (Signals, Standalone Components).

## 🛠️ Tech Stack

**Frontend**

- **Framework:** Angular 17+
- **State Management:** Angular Signals
- **UI Library:** Angular Material & CDK (Drag & Drop)
- **Styling:** SCSS

**Backend**

- **Framework:** .NET 9 Web API
- **Language:** C#
- **AI Integration:** Google Gemini (via `Mscc.GenerativeAI`)
- **Database:** PostgreSQL (running in Docker)
- **ORM:** Entity Framework Core

## ⚙️ Prerequisites

Before you begin, ensure you have the following installed:

1. **Node.js** (LTS version)
2. **.NET 9 SDK**
3. **Docker Desktop** (must be running)
4. **Google Gemini API Key** (Get one from Google AI Studio)

## 🚀 Installation & Setup Guide

Follow these steps to run the project locally.

### 1. Clone the Repository

```bash
git clone [https://github.com/albert0891/OfferMagnet.git](https://github.com/albert0891/OfferMagnet.git)
cd OfferMagnet
```

### 2. Backend Setup (API & Database)

**Step A: Secure your AI API Key**
We use .NET User Secrets to keep your API key safe (not committed to Git).

```bash
cd JobTracker.Api

# 1. Initialize User Secrets for the project (Crucial Step!)
dotnet user-secrets init

# 2. Set your Google Gemini API Key
dotnet user-secrets set "Gemini:ApiKey" "YOUR_ACTUAL_GOOGLE_API_KEY_HERE"
```

**Step B: Start the Database**
Make sure Docker Desktop is running, then start the PostgreSQL container.

```bash
# Still in the backend folder
docker-compose up -d
```

**Step C: Run the API**

```bash
dotnet watch run
```

The Backend API will start (usually at `https://localhost:7xxx`). Keep this terminal open.

### 3. Frontend Setup (Web App)

Open a new terminal window and navigate to the frontend folder.

```bash
cd job-tracker-web

# 1. Install dependencies
npm install

# 2. Run the application
ng serve -o
```

The application will open automatically at `http://localhost:4200`.

## 📌 Usage

- **Kanban Board:** Open `http://localhost:4200`. You will see seeded data (if configured) or an empty board.
- **Add Job:** Use the API (Swagger) or UI to add jobs.
- **Drag & Drop:** Move cards between columns to update their status automatically.
- **AI Analysis:** Click the "Analyze AI" button on a job card (in the 'Applied' column) to trigger the Gemini analysis.
- **Swagger Documentation:** Visit `https://localhost:7xxx/swagger` to test backend endpoints directly.

## 🤝 Contributing

1. Fork the repository.
2. Create a feature branch (`git checkout -b feat/amazing-feature`).
3. Commit your changes (`git commit -m 'feat: Add amazing feature'`).
4. Push to the branch (`git push origin feat/amazing-feature`).
5. Open a Pull Request.
