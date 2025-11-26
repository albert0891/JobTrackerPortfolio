🚀 AI-Powered Job Application TrackerA full-stack, AI-integrated application designed to streamline the job search process. This project features a drag-and-drop Kanban board for application tracking and leverages Google Gemini AI to analyze resumes against job descriptions.✨ Key FeaturesSmart Kanban Board: Manage job applications (Applied, Interviewing, Offer, Rejected) using an interactive drag-and-drop interface powered by Angular CDK.AI Analysis Agent: Integrates Google Gemini AI to analyze job descriptions against a user's resume, providing a match score and keyword optimization strategy.Modern Architecture: * Frontend: Angular 17+ using Signals for reactive state management and Standalone Components.Backend: .NET 9 Web API following the Repository/Service Pattern and Clean Architecture principles.Containerized Database: Uses PostgreSQL running in Docker for consistent data persistence across environments.🛠️ Tech StackFrontendFramework: Angular 17+State Management: Angular SignalsUI Component Library: Angular Material (CDK Drag & Drop)Styling: SCSSBackendFramework: .NET 9 Web APILanguage: C#AI Integration: Google Gemini (via Mscc.GenerativeAI.Google)Database ORM: Entity Framework CoreAPI Documentation: Swagger / OpenAPIInfrastructureDatabase: PostgreSQL (Docker)Containerization: Docker Compose⚙️ PrerequisitesBefore you begin, ensure you have the following installed:Docker Desktop (Must be running).NET 9 SDKNode.js (LTS Version)Google Gemini API Key (Get one from Google AI Studio)🚀 Installation & Setup GuideFollow these steps to get the project running on your local machine.1. Clone the Repositorygit clone [https://github.com/albert0891/JobTrackerPortfolio.git](https://github.com/albert0891/JobTrackerPortfolio.git)
cd JobTrackerPortfolio
2. Start the Database (Docker)We use Docker to spin up a PostgreSQL instance instantly without manual installation.cd JobTracker.Api
docker-compose up -d
Wait a few seconds for the database to initialize.3. Setup the Backend (.NET API)Step A: Configure SecretsWe do not commit API keys to GitHub. You must set your Google Gemini API Key locally.(Make sure you are in the JobTracker.Api folder)dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "YOUR_ACTUAL_GOOGLE_API_KEY_HERE"
Step B: Restore Dependencies & DatabaseThis downloads all required NuGet packages (PostgreSQL driver, AI SDK, etc.) and creates the database schema.dotnet restore
dotnet ef database update
Step C: Trust HTTPS Certificate (Local Development Only)This prevents browser security errors when connecting to the API.dotnet dev-certs https --trust
Step D: Run the APIdotnet watch run
The Backend API will start (usually at https://localhost:7xxx).You can access the Swagger UI at https://localhost:7xxx/swagger to test endpoints.4. Setup the Frontend (Angular App)Open a new terminal window and navigate to the frontend directory.Step A: Install Dependenciescd job-tracker-web
npm install
Step B: Run the Appng serve -o
The application will automatically open in your browser at http://localhost:4200.🧪 How to UseKanban Board: Open http://localhost:4200. You can add jobs (via Swagger for now) and drag them between columns (Applied, Interviewing, etc.).AI Analysis (Backend Test): * Go to the Swagger UI (https://localhost:7xxx/swagger).Find the POST /api/Ai/analyze/{jobId} endpoint.Enter a valid Job ID (e.g., 1).Execute to see Google Gemini analyze the job description!📂 Project StructureJobTrackerPortfolio/
├── JobTracker.Api/          # .NET 9 Backend
│   ├── Controllers/         # API Endpoints
│   ├── Models/              # Database Entities & DTOs
│   ├── Services/            # Business Logic (AI, Job Service)
│   └── docker-compose.yml   # Database Configuration
│
└── job-tracker-web/         # Angular Frontend
    ├── src/app/
    │   ├── components/      # UI Components (Kanban)
    │   ├── models/          # TypeScript Interfaces
    │   └── services/        # API Communication (Signals)
