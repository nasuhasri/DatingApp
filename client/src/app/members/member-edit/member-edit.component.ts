import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { MemberService } from 'src/app/_service/member.service';
import { AccountService } from 'src/app/_services/account.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  member: Member | undefined;
  user: User | null = null;

  constructor(private accountService: AccountService, private memberService: MemberService) {
    // take 1: as soon as we have this user, the request is the completed and dont need to unsubscribe
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.user = user
    })
  }
  
  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    if (!this.user) return;

    this.memberService.getMember(this.user.username).subscribe({
      next: member => this.member = member
    })
  }
}
