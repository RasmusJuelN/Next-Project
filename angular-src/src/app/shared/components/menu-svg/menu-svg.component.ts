
import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-menu-svg',
    imports: [],
    templateUrl: './menu-svg.component.html',
    styleUrl: './menu-svg.component.css'
})
export class MenuSvgComponent {
  @Input() logoType: string = 'menu';
}
