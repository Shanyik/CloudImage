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

  ngOnInit(): void {
    this.pingAuth();
  }

  pingAuth() {
    this.cookieService.get('login') ? this.router.navigate(['/dashboard']) : '';
  }

  login() {
    this.apiService.loginUser(this.credentials).subscribe(
      (response) => {
        this.cookieService.set('login', 'true');
        this.router.navigate(['/dashboard']);
        location.reload();
      },
      (error) => {
        console.error(error);
        // Handle error, e.g., show an error message
      }
    );
  }
}
