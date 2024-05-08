import { Component, OnInit } from '@angular/core';
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

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getApiKeyInfo().subscribe(
      (response: any) => {
        console.log(response);
        this.usedStorage = response.usedStorageGB;
        this.allocatedStorage = response.allocatedStorageGB;
      },
      (error: any) => {
        if (error.status !== 404) {
          console.error('Error fetching API key info:', error);
        }
      }
    );
  }
}
