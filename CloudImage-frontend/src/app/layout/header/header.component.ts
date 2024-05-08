import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  isLoggedIn: boolean = false;

  constructor(
    private apiService: ApiService,
    private router: Router,
    private cookieService: CookieService
  ) {}

  ngOnInit(): void {
    this.apiService.pingAuth().subscribe((response) => {
      if (response.email != null) {
        this.isLoggedIn = true;
      }
    });
  }

  logout(): void {
    this.apiService.logout().subscribe(() => {});
    this.cookieService.delete('login');
    this.router.navigate(['']);
    setTimeout(() => {
      location.reload();
    }, 100);
  }
}
