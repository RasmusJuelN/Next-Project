import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { User, ActiveQuestionnaire } from '../../../models/questionare';
import { AdminDashboardService } from '../../../services/dashboard/admin-dashboard.service';
import { AppAuthService } from '../../../services/auth/app-auth.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['../shared-dashboard-styles.css','./admin-dashboard.component.css']
})
export class AdminDashboardComponent {
  
}
