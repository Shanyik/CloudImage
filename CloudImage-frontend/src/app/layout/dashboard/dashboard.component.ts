import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  apiKey!: string;

  constructor(private cookieService: CookieService) {}

  ngOnInit(): void {
    this.apiKey = this.cookieService.get('apiKey');
  }
}
