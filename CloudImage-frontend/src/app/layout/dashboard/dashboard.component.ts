import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { ApiService } from '../../services/api.service';
import { CommonModule } from '@angular/common';
import { ProgressBarModule } from 'primeng/progressbar';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ProgressBarModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  apiKey!: string;
  usedStorage!: number;
  allocatedStorage!: number;

  constructor(
    private apiService: ApiService,
    private cookieService: CookieService
  ) {}

  ngOnInit(): void {
    this.apiKey = this.cookieService.get('apiKey');
    if (this.apiKey) {
      this.apiService.getApiKeyInfo(this.apiKey).subscribe(
        (response: any) => {
          console.log(response.usedStorageGB);
          this.usedStorage = response.usedStorageGB;
          this.allocatedStorage = response.allocatedStorageGB;
        },
        (error: any) => {
          console.error('Error fetching API key info:', error);
          // Handle error
        }
      );
    }
  }
}
