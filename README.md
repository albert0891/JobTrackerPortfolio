# 🚀 AI-Powered Job Application Tracker (Full-Stack)

A modern, full-stack job tracking application featuring a Kanban board interface built with Angular and a robust backend API developed with .NET 9. The project integrates an AI service (via OpenAI) to provide smart analysis for job applicants.

## 🌟 Features

- **Kanban Board:** Intuitive drag-and-drop interface (using Angular CDK) to manage job application stages (Applied, Interviewing, Rejected, etc.).
- **AI Match Analysis:** Backend service that compares a user's resume against a job description, providing a match score and suggested keywords for improvement.
- **Persistent Data:** Uses a Dockerized PostgreSQL database for reliable data storage.
- **Modern Stack:** Built using Angular Signals, .NET 9 Web API, and a clean Service Layer architecture.

## 🛠️ Tech Stack

**Frontend (Client)**

- **Framework:** Angular 17+ (Standalone Components, Signals)
- **UI/UX:** Angular Material (CDK)
- **State:** Local Signals

**Backend (API)**

- **Framework:** .NET 9 Web API
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core (EF Core)
- **AI Integration:** OpenAI Client SDK
- **Architecture:** Repository/Service Pattern

**Tooling & Environment**

- **Containerization:** Docker & Docker Compose
- **Language:** C# and TypeScript

## ⚙️ Prerequisites

Before running the project, you need:

1.  .NET 9 SDK
2.  Node.js (LTS version)
3.  Docker Desktop
4.  An OpenAI API Key (required for the AI features)

## 💻 Installation & Setup

1.  **Clone the Repository:**

    ```bash
    git clone [YOUR_GITHUB_REPO_URL]
    cd JobTrackerPortfolio
    ```

2.  **Secure API Key (User Secrets):**
    Navigate to the API folder and set your secret key (replace `sk-...` with your actual key):

    ```bash
    cd JobTracker.Api
    dotnet user-secrets set "OpenAI:ApiKey" "sk-xxxxxxxxxxxxxxxxxxxxxxxx"
    ```

3.  **Start the Database:**
    Start the PostgreSQL container using Docker Compose:

    ```bash
    docker-compose up -d
    ```

4.  **Run Backend (API):**

    ```bash
    dotnet run
    ```

    _The API will run on a port like `https://localhost:7xxx`._

5.  **Run Frontend (Client):**
    Open a new terminal, navigate to the client folder, install dependencies, and run:
    ```bash
    cd ../job-tracker-web
    npm install
    ng serve -o
    ```

The application will open automatically in your browser at `http://localhost:4200`.

## 📌 Usage

Access the Kanban board to drag job cards between columns. Use the Swagger UI on the backend port (`https://localhost:7xxx/swagger`) to verify API endpoints and test the `POST /api/ai/analyze/{jobId}` endpoint.
