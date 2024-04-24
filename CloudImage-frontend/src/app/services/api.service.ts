import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private baseUrl = 'http://localhost:5246';

  constructor(private http: HttpClient) {}

  generateApiKey(): Observable<any> {
    return this.http.post(`${this.baseUrl}/Image/GenerateKey`, {});
  }

  getRemainingSlots(): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/Image/remainingSlots`);
  }
}
