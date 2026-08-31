import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface TeamMember {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  designation: string;
  department: string;
}

@Injectable({
  providedIn: 'root',
})
export class TeamService {
  constructor() {}

  getTeamMembers(): Observable<TeamMember[]> {
    // TODO: Implement backend call
    return new Observable((observer) => {
      observer.next([
        { id: '1', firstName: 'Alice', lastName: 'Smith', email: 'alice@example.com', designation: 'Senior Engineer', department: 'Engineering' },
        { id: '2', firstName: 'Bob', lastName: 'Johnson', email: 'bob@example.com', designation: 'QA Engineer', department: 'Quality' },
      ]);
      observer.complete();
    });
  }

  getTeamMemberDetail(id: string): Observable<TeamMember> {
    // TODO: Implement backend call
    return new Observable((observer) => {
      observer.next({ id, firstName: 'Alice', lastName: 'Smith', email: 'alice@example.com', designation: 'Senior Engineer', department: 'Engineering' });
      observer.complete();
    });
  }
}
