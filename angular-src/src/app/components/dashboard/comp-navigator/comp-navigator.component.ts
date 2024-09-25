import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-comp-navigator',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './comp-navigator.component.html',
  styleUrl: './comp-navigator.component.css'
})
export class CompNavigatorComponent {

  userRole = "";
  authService = inject(AuthService)

  ngOnInit(): void {
    // Assume authService has a method to get the user role
    const role = this.authService.getUserRole();  // Example method to fetch user role
    if (role){
      this.userRole = role
    }
  }

}
