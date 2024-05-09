import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { CommonModule } from '@angular/common';
import { ProgressBarModule } from 'primeng/progressbar';
import { FileUploadModule } from 'primeng/fileupload';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ProgressBarModule, FileUploadModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  apiKey!: string;
  link!: string;
  usedStorage!: number;
  allocatedStorage!: number;
  files: File[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit(): void {
    this.apiService.getApiKeyInfo().subscribe(
      (response: any) => {
        this.link = location.origin.replace('4200', '5246/Api/Image');
        console.log(location.origin);
        this.apiKey = response.key;
        this.usedStorage =
          Math.round(response.usedStorageGB * 1024 * 100) / 100;
        this.allocatedStorage = response.allocatedStorageGB * 1024;
      },
      (error: any) => {
        if (error.status !== 404) {
          console.error('Error fetching API key info:', error);
        }
      }
    );
  }

  onFileChange(event: any) {
    const selectedFiles: FileList = event.target.files;
    for (let i = 0; i < selectedFiles.length; i++) {
      this.files.push(selectedFiles[i]);
    }
  }

  onUpload() {
    const formData = new FormData();
    for (let i = 0; i < this.files.length; i++) {
      formData.append('files[]', this.files[i]);
    }

    this.apiService
      .uploadImage(formData, this.apiKey)
      .subscribe((response: any) => {
        location.reload();
      });
  }
}
