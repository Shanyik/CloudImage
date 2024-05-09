import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { LandingComponent } from '../layout/landing/landing.component';
import { DescriptionComponent } from '../layout/description/description.component';
import { SeparatorComponent } from '../layout/separator/separator.component';
import { VarietyComponent } from '../layout/variety/variety.component';
import { HowtouseComponent } from '../layout/howtouse/howtouse.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    ButtonModule,
    FormsModule,
    LandingComponent,
    DescriptionComponent,
    SeparatorComponent,
    VarietyComponent,
    HowtouseComponent,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  apiKey!: string;
  existingApiKey!: string;
  errorMessage!: string;

  constructor() {}
}
