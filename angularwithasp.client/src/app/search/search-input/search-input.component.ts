import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-input',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-input.component.html',
  styleUrl: './search-input.component.css'
})
export class SearchInputComponent {
  @Input() initVal: string = "";
  @Input() placeholder: string = "Search";
  @Output() parentHandleInputChange = new EventEmitter<string>();

  onInputChange(val: string) {
    this.parentHandleInputChange.emit(val);
  }

  onClear() {
    this.parentHandleInputChange.emit("");
  }
}

