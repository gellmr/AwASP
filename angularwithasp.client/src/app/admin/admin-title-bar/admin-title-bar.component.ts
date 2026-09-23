import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-title-bar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-title-bar.component.html',
  styleUrl: './admin-title-bar.component.css'
})
export class AdminTitleBarComponent {
  @Input() titleText: string = "Title Here";
  @Input() construction: boolean = false;
}

