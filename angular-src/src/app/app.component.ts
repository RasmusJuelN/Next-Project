import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule],
  templateUrl: 'app.component.html',
  styleUrl: 'app.component.css'
})
export class AppComponent {
  title = 'Start Screen';
  tokenExists = false;

  ngOnInit() {
    this.tokenExists = localStorage.getItem('token') !== null;
  }

  deleteToken() {
    localStorage.removeItem('token');
    this.tokenExists = false;
    alert('Token deleted');
  }

}
