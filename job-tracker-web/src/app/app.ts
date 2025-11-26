// src/app/app.ts (Main Application Component)

import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
// Import the component we just created
import { KanbanBoardComponent } from './components/kanban-board/kanban-board';

@Component({
  selector: 'app-root',
  standalone: true,
  // Make sure KanbanBoardComponent is in the imports array
  imports: [CommonModule, KanbanBoardComponent],
  template: `
    <header>
      <h1>OfferMagnet</h1>
    </header>
    <main>
      <app-kanban-board></app-kanban-board>
    </main>
  `,
  styleUrls: ['./app.scss'],
})
export class App {
  title = 'job-tracker-web';
}
