import { Component, EventEmitter, Output, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../services/cart.service';

@Component({
  selector: 'app-drag-drop-user-pic-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './drag-drop-user-pic-modal.component.html',
  styleUrl: './drag-drop-user-pic-modal.component.css'
})
export class DragDropUserPicModalComponent {
  @Output() onSuccess = new EventEmitter<{ idval: string, picture: string, usertype: string }>();

  show = false;
  idval: string | null = null;
  usertype: string | null = null;
  defaultCloudGraphic = '/graphics/cloud-upload.png';
  cloudGraphic = this.defaultCloudGraphic;

  private http = inject(HttpClient);
  public cartService = inject(CartService);
  private cdr = inject(ChangeDetectorRef);

  get isGoogleSignIn(): boolean {
    return this.cartService.user ? !!this.cartService.user.isGoogleSignIn : false;
  }

  showModal(idv: string, utype: string) {
    this.cloudGraphic = this.defaultCloudGraphic;
    this.idval = idv;
    this.usertype = utype;
    this.show = true;
    this.cdr.markForCheck();
  }

  hideModal() {
    this.show = false;
    this.cdr.markForCheck();
  }

  handleClose() {
    this.hideModal();
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      const file = event.dataTransfer.files[0];
      this.uploadFile(file);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.uploadFile(file);
    }
  }

  uploadFile(file: File) {
    if (!file) return;

    const url = `/api/admin-userpic?idval=${this.idval}&usertype=${this.usertype}`;
    const formData = new FormData();
    formData.append('file', file);

    this.http.post<any>(url, formData).subscribe({
      next: (response) => {
        console.log('File uploaded successfully!', response);
        this.cloudGraphic = response.picture;
        this.onSuccess.emit({
          idval: response.idsave,
          picture: response.picture,
          usertype: this.usertype || ''
        });
        this.cdr.markForCheck();
        setTimeout(() => {
          this.handleClose();
        }, 800);
      },
      error: (error) => {
        console.error('Request failed.', error);
        this.handleClose();
      }
    });
  }
}

