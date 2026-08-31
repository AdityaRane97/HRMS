import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TeamService } from './team.service';

@Component({
  selector: 'app-team',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  teamMembers: any[] = [];
  loading = true;
  error: string | null = null;

  constructor(private teamService: TeamService) {}

  ngOnInit(): void {
    this.loadTeamMembers();
  }

  private loadTeamMembers(): void {
    this.teamService.getTeamMembers().subscribe({
      next: (members) => {
        this.teamMembers = members;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load team members';
        this.loading = false;
      },
    });
  }
}
