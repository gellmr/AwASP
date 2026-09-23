import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-my-orders-show-account-id',
  standalone: true,
  templateUrl: './my-orders-show-account-id.component.html',
  styleUrl: './my-orders-show-account-id.component.css'
})
export class MyOrdersShowAccountIdComponent {
  @Input() accType: string = '';
  @Input() idval: string = '';
}
