# ✈️ OfferMagnet

**OfferMagnet** is a smart, full-stack career management platform designed to streamline your job search. It combines a visual Kanban workflow with Generative AI (Google Gemini) to act as your personal career coach, analyzing your fit for every role you apply to.

## 🌟 Key Features

* **🤖 AI-Powered Analysis:** Instantly compares your resume against job descriptions using Google Gemini. It provides a match score, highlights your strengths, and identifies missing keywords.
* **📋 Kanban Workflow:** Drag-and-drop interface (Angular CDK) to visually track applications through stages (Applied, Interviewing, Offer, Rejected).
* **🚀 Modern Tech Stack:** Built with **.NET 9 Web API** and **Angular 17+ (Signals)** for high performance and maintainability.
* **🐳 Containerized DB:** Uses Dockerized PostgreSQL for reliable, portable data persistence.

## 🛠️ Technology Stack

* **Frontend:** Angular 18 (Standalone Components, Signals), Angular Material, SCSS
* **Backend:** .NET 9 Web API, Entity Framework Core, Mscc.GenerativeAI (Gemini SDK)
* **Database:** PostgreSQL (via Docker Compose)
* **Tools:** Visual Studio Code, Swagger UI

## 🚀 Getting Started

### Prerequisites
* .NET 9 SDK
* Node.js (LTS)
* Docker Desktop
* Google AI Studio API Key

### Installation

1.  **Clone the Repository**
    ```bash
    git clone [https://github.com/albert0891/JobTrackerPortfolio.git](https://github.com/albert0891/JobTrackerPortfolio.git)
    cd JobTrackerPortfolio
    ```

2.  **Backend Setup (With User Secrets)**
    Navigate to the API folder and configure your Gemini API key.
    ```bash
    cd JobTracker.Api
    
    # Initialize secrets storage
    dotnet user-secrets init
    
    # Set your Google API Key
    dotnet user-secrets set "Gemini:ApiKey" "YOUR_GOOGLE_API_KEY_HERE"
    
    # Start the Database
    docker-compose up -d
    
    # Run the API
    dotnet watch run
    ```
    *Trust the dev certificate if prompted:* `dotnet dev-certs https --trust`

3.  **Frontend Setup**
    Open a new terminal.
    ```bash
    cd job-tracker-web
    npm install
    ng serve -o
    ```

4.  **Usage**
    * Open `http://localhost:4200`.
    * Click the **"Analyze"** button on any job card.
    * Watch OfferMagnet generate a personalized analysis of your application!

## 🤝 Contributing
This is a personal portfolio project demonstrating Full-Stack & AI integration capabilities.

---
*Built by Albert*