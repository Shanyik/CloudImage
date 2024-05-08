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

  getApiKeyInfo(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/api/user`, {
      withCredentials: true,
    });
  }

  pingAuth(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/api/pingauth`, {
      withCredentials: true,
    });
  }

  validateApiKey(apiKey: string): Observable<boolean> {
    return this.http.get<any>(
      `${this.baseUrl}/Image/isValidKey?apiKey=${apiKey}`
    );
  }

  registerUser(userDetails: any): Observable<any> {
    return this.http.post<any>(
      `${this.baseUrl}/api/User/add-user`,
      userDetails
    );
  }

  loginUser(credentials: any): Observable<any> {
    return this.http.post<any>(
      `${this.baseUrl}/api/account/login?useCookies=true`,
      credentials,
      { withCredentials: true }
    );
  }

  logout(): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/api/account/logout`, '', {
      withCredentials: true,
    });
  }
}
