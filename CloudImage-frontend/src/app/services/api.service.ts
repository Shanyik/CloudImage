import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private baseUrl = environment.API_BASE_URL;

  constructor(private http: HttpClient) {}

  generateApiKey(): Observable<any> {
    return this.http.post(`${this.baseUrl}/Image/GenerateKey`, {});
  }

  getRemainingSlots(): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/Image/remainingSlots`);
  }

  getApiKeyInfo(apiKey: string): Observable<any> {
    return this.http.get<any>(
      `${this.baseUrl}/Image/apiKeyInfo?apiKey=${apiKey}`
    );
  }

  validateApiKey(apiKey: string): Observable<boolean> {
    return this.http.get<any>(
      `${this.baseUrl}/Image/isValidKey?apiKey=${apiKey}`
    );
  }
}
