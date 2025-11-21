import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

/**
 * MenuSvgComponent
 *
 * Displays an SVG icon depending on the `logoType` input:
 * - `"menu"` → hamburger menu icon.
 * - `"close"` → close (X) icon.
 * - Any other value → fallback warning/error icon (red circle).
 *
 * Typically used in navigation components (e.g., header or sidebar toggles).
 *
 * @example
 * ```html
 * <app-menu-svg logoType="menu"></app-menu-svg>
 * <app-menu-svg logoType="close"></app-menu-svg>
 * ```
 */
@Component({
    selector: 'app-menu-svg',
    imports: [CommonModule],
    templateUrl: './menu-svg.component.html',
    styleUrl: './menu-svg.component.css'
})
export class MenuSvgComponent {
  @Input() logoType: string = 'menu';
}
