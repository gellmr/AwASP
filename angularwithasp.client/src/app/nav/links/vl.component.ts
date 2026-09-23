import { Component } from '@angular/core';

@Component({
  selector: 'app-vl',
  standalone: true,
  template: '<div class="vl">&nbsp;</div>',
  styles: [`
    .vl {
      border-left: 1px solid rgb(65 93 135);
      display: inline-block;
      margin-left: 3px;
      width: 2px;
    }
  `]
})
export class VlComponent {}
