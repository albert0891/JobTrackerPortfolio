# 🚀 Job Application Tracker (Full-Stack)

A modern, full-stack job tracking application featuring a Kanban board interface built with Angular and a robust backend API developed with .NET 9.

## 🌟 Features

- **Kanban Board:** Intuitive drag-and-drop interface (using Angular CDK) to manage job application stages (Applied, Interviewing, Offer, Rejected).
- **Persistent Data:** Uses a Dockerized PostgreSQL database for reliable data storage.
- **Modern State Management:** Implements Angular Signals for efficient reactive data handling.
- **Architecture:** Built with a clean Service Layer architecture on the backend.

## 🛠️ Tech Stack

**Frontend (Client)**

- **Framework:** Angular 17+ (Standalone Components, Signals)
- **UI/UX:** Angular Material (CDK)
- **State:** Local Signals

**Backend (API)**

- **Framework:** .NET 9 Web API
- **Database:** PostgreSQL
- **ORM:** Entity Framework Core (EF Core)
- **Architecture:** Repository/Service Pattern

**Tooling & Environment**

- **Containerization:** Docker & Docker Compose
- **Language:** C# and TypeScript

## ⚙️ Prerequisites

Before running the project, you need:

1.  .NET 9 SDK
2.  Node.js (LTS version)
3.  Docker Desktop

## 💻 Installation & Setup

1.  **Clone the Repository:**

    ```bash
    git clone [YOUR_GITHUB_REPO_URL]
    cd JobTrackerPortfolio
    ```

2.  **Start the Database:**
    Start the PostgreSQL container using Docker Compose:

    ```bash
    cd JobTracker.Api
    docker-compose up -d
    ```

3.  **Run Backend (API):**
    Ensure you trust the development certificate first:

    ```bash
    dotnet dev-certs https --trust
    ```

    Then start the server:

    ```bash
    dotnet watch run
    ```

    _The API will typically run on `https://localhost:7xxx`._

4.  **Run Frontend (Client):**
    Open a new terminal, navigate to the client folder, install dependencies, and run:
    ```bash
    cd ../job-tracker-web
    npm install
    ng serve -o
    ```

The application will open automatically in your browser at `http://localhost:4200`.

## 📌 Usage

- Access the **Kanban board** at `http://localhost:4200` to verify the frontend.
- Use the **Swagger UI** on the backend port (e.g., `https://localhost:7251/swagger`) to verify API endpoints and add initial data using `POST /api/JobApplications`.
