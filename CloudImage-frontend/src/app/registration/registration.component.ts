import { HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';
import {
  FormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../services/api.service';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-registration',
  standalone: true,
  imports: [FormsModule, HttpClientModule, ButtonModule, CommonModule],
  templateUrl: './registration.component.html',
  styleUrl: './registration.component.scss',
})
export class RegistrationComponent {
  model: any = {};
  remainingSlots!: number;
  registrationForm!: FormGroup;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.getRemainingSlots();
  }

  initForm(): void {
    this.registrationForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  register() {
    this.apiService.registerUser(this.model).subscribe(
      (response) => {
        this.router.navigate(['/login']);
      },
      (error) => {
        console.error(error);
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
