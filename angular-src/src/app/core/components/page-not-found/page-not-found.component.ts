import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

/**
 * PageNotFoundComponent
 *
 * Displays a simple 404 error page with:
 * - A large "404" title.
 * - An error message.
 * - A button to navigate back to the home page.
 *
 * This component is used as a **catch-all route** (`path: '**'`)
 * to handle undefined or invalid URLs gracefully.
 *
 * @example
 * ```ts
 * export const routes: Routes = [
 *   { path: '**', component: PageNotFoundComponent }
 * ];
 * ```
 */
@Component({
    selector: 'app-page-not-found',
    imports: [RouterLink],
    templateUrl: './page-not-found.component.html',
    styleUrl: './page-not-found.component.css'
})
export class PageNotFoundComponent {

}
