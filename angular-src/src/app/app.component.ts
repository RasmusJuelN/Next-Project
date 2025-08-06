
import { Component, inject, OnInit } from '@angular/core';
import { RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { Observable, of } from 'rxjs';
import {HeaderComponent } from './core/components/app-header/header.component';

@Component({
    selector: 'app-root',
    imports: [RouterModule, RouterOutlet, HeaderComponent],
    templateUrl: 'app.component.html',
    styleUrls: ['app.component.css']
})
export class AppComponent {

}
