import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { MessageService } from 'primeng/api';
import { CommonModule } from '@angular/common';
import { PaginatorModule } from 'primeng/paginator';
import { ButtonModule } from 'primeng/button';
import { ImageModule } from 'primeng/image';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-image-viewer',
  standalone: true,
  templateUrl: './image-viewer.component.html',
  styleUrls: ['./image-viewer.component.scss'],
  imports: [
    CommonModule,
    PaginatorModule,
    ButtonModule,
    ImageModule,
    ToastModule,
  ],
  providers: [MessageService],
})
export class ImageViewerComponent implements OnInit {
  images: any[] = [];
  paginatedImages: any[] = [];
  totalRecords: number = 0;
  rowsPerPage: number = 12;
  apiKey!: string;

  constructor(
    private apiService: ApiService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loadImages();
  }

  loadImages() {
    this.apiService.getApiKeyInfo().subscribe(
      (response: any) => {
        this.images = response.images;
        this.apiKey = response.key;
        this.totalRecords = this.images.length;
        this.paginate({ first: 0, rows: this.rowsPerPage });
      },
      (error: any) => {
        console.error('Error fetching images:', error);
      }
    );
  }
  paginate(event: any) {
    const start = event.first;
    const end = start + event.rows;
    this.paginatedImages = this.images.slice(start, end);
  }

  deleteImage(imageUrl: string) {
    console.log(imageUrl);
    const imageUrls = Array.of(imageUrl);
    this.apiService.deleteImage(imageUrls, this.apiKey).subscribe(
      () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Image Deleted',
          detail: 'The image has been deleted.',
        });
        this.loadImages();
      },
      (error: any) => {
        console.error('Error deleting image:', error);
        this.messageService.add({
          severity: 'error',
          summary: 'Delete Failed',
          detail: 'Failed to delete the image. Please try again.',
        });
      }
    );
  }
}
