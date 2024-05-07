import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { CookieService } from 'ngx-cookie-service';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, ButtonModule, FormsModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent implements OnInit {
  remainingSlots!: number;
  apiKey!: string;
  existingApiKey!: string;
  errorMessage!: string;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private cookieService: CookieService
  ) {}

  ngOnInit(): void {
    this.getRemainingSlots();
  }

  goToRegistration() {
    this.router.navigate(['/registration']);
  }

  useExistingApiKey() {
    this.apiService.validateApiKey(this.existingApiKey).subscribe(
      (isValid) => {
        if (isValid) {
          this.cookieService.set('apiKey', this.existingApiKey);
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage = 'Invalid API key';
        }
      },
      (error) => {
        console.error('Error validating API key:', error);
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
