import { Component, ViewEncapsulation } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SiteNavBarComponent } from '../../nav/site-nav-bar.component';
import { FooterComponent } from '../../shop/footer/footer.component';

@Component({
  selector: 'app-login-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, SiteNavBarComponent, FooterComponent],
  templateUrl: './login-layout.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrl: './login-layout.component.css'
})
export class LoginLayoutComponent {
}
