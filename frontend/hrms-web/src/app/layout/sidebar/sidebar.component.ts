import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SidebarItem } from './sidebar.model';
import { AuthService } from '@core/auth/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent implements OnInit {
  @Input() isOpen = true;

  menuItems: SidebarItem[] = [
    { label: 'Dashboard', route: '/dashboard', icon: '📊' },
    { label: 'Profile', route: '/profile', icon: '👤' },
    { label: 'Team', route: '/team', icon: '👥' },
    { label: 'Salary', route: '/salary', icon: '💰' },
    { label: 'Timesheet', route: '/timesheet', icon: '⏱️' },
    { label: 'Documents', route: '/documents', icon: '📄' },
    { label: 'Policies', route: '/policies', icon: '📋' },
    { label: 'Rewards', route: '/rewards', icon: '🏆' },
    { label: 'Tasks', route: '/tasks', icon: '✓' },
    { label: 'Chatbot', route: '/chatbot', icon: '🤖' },
    { label: 'Administration', route: '/administration', icon: '⚙️' },
  ];

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // TODO: Filter menu items based on user role
  }

  isMenuItemVisible(item: SidebarItem): boolean {
    if (!item.requiredRoles || item.requiredRoles.length === 0) {
      return true;
    }
    // TODO: Check if user has required role
    return true;
  }
}
