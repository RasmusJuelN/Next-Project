import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-menu-svg',
    imports: [CommonModule],
    templateUrl: './menu-svg.component.html',
    styleUrl: './menu-svg.component.css'
})
export class MenuSvgComponent {
  @Input() logoType: string = 'menu';
}
