import { Component, Input, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common';

@Component({
  selector: 'app-back-link',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './back-link.component.html',
  styleUrl: './back-link.component.css'
})
export class BackLinkComponent {
  @Input() textPos: string = 'left';
  @Input() btnClasses: string = 'btn btn-light';

  private location = inject(Location);

  handleGoBack() {
    this.location.back();
  }
}
