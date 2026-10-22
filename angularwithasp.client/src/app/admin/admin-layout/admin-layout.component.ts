import { Component, ViewEncapsulation  } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SiteNavBarComponent } from '../../nav/site-nav-bar.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, SiteNavBarComponent],
  templateUrl: './admin-layout.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent {
}

