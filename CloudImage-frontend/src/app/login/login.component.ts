import { Component } from '@angular/core';
import { ApiService } from '../services/api.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, HttpClientModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  credentials: any = {};

  constructor(
    private apiService: ApiService,
    private router: Router,
    private cookieService: CookieService
  ) {}

  login() {
    this.apiService.loginUser(this.credentials).subscribe(
      (response) => {
        console.log(response);
        // Handle success, e.g., store token in local storage and redirect
        this.cookieService.set('token', response.accessToken);
        this.router.navigate(['/dashboard']); // Redirect to dashboard after successful login
      },
      (error) => {
        console.error(error);
        // Handle error, e.g., show an error message
      }
    );
  }
}
