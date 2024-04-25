import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  remainingSlots!: number;
  apiKey!: string;

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.getRemainingSlots();
  }

  generateApiKey() {
    this.apiService.generateApiKey().subscribe(
      (response: any) => {
        console.log('API Key:', response.apiKey);
        this.apiKey = response.apiKey;
        this.getRemainingSlots();
      },
      (error: any) => {
        console.error('Error generating API key:', error);
        // Handle error
      }
    );
  }

  getRemainingSlots() {
    this.apiService.getRemainingSlots().subscribe(
      (response: number) => {
        this.remainingSlots = response;
      },
      (error: any) => {
        console.error('Error getting remaining slots:', error);
        // Handle error
      }
    );
  }
}
