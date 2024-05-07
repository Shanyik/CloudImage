import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [FormsModule, HttpClientModule],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.scss',
})
export class RegistrationComponent {
  model: any = {};

  constructor(
    private http: HttpClient,
    private router: Router,
    private apiService: ApiService
  ) {}

  register() {
    this.apiService.registerUser(this.model).subscribe(
      (response) => {
        console.log(response);
        // Handle success, e.g., show a success message
        this.router.navigate(['/login']); // Redirect to login page after successful registration
      },
      (error) => {
        console.error(error);
        // Handle error, e.g., show an error message
      }
    );
  }
}
