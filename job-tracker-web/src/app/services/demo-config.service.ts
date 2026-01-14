import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DemoConfig {
  isEnabled: boolean;
  cleanupIntervalMinutes: number;
}

@Injectable({
  providedIn: 'root'
})
export class DemoConfigService {
  private apiUrl = `${environment.apiUrl}/config/demo`;

  constructor(private http: HttpClient) { }

  getDemoConfig(): Observable<DemoConfig> {
    return this.http.get<DemoConfig>(this.apiUrl);
  }
}
