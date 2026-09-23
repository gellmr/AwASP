import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-search-box',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-search-box.component.html',
  styleUrl: './product-search-box.component.css'
})
export class ProductSearchBoxComponent {
  @Input() searchTerm: string = '';
  @Output() searchChange = new EventEmitter<string>();

  onSearchInput(event: Event) {
    const inputElement = event.target as HTMLInputElement;
    this.searchChange.emit(inputElement.value);
  }

  clearSearch() {
    this.searchChange.emit('');
  }
}
