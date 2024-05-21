import { Component } from '@angular/core';
import { ApiService } from '../services/api.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { ButtonModule } from 'primeng/button';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    HttpClientModule,
    ButtonModule,
    ToastModule,
    CommonModule,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
  providers: [MessageService],
})
export class LoginComponent {
  credentials: any = {};

  constructor(
    private apiService: ApiService,
    private router: Router,
    private cookieService: CookieService,
    private messageService: MessageService
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
        this.messageService.add({
          severity: 'error',
          summary: 'Login Failed',
          detail: 'The user / password is invalid',
        });
      }
    );
  }
}
