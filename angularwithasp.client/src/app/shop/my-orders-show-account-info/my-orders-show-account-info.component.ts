import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyOrdersShowAccountIdComponent } from '../my-orders-show-account-id/my-orders-show-account-id.component';

@Component({
  selector: 'app-my-orders-show-account-info',
  standalone: true,
  imports: [CommonModule, MyOrdersShowAccountIdComponent],
  templateUrl: './my-orders-show-account-info.component.html',
  styleUrl: './my-orders-show-account-info.component.css'
})
export class MyOrdersShowAccountInfoComponent {
  @Input() accType: string = '';
  @Input() idval: string = '';
  @Input() fullname: string = '';
  @Input() email: string = '';
  @Input() devMode: boolean = false;
}
