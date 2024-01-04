import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
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
  @ViewChild('editForm') editForm: NgForm | undefined;
  // activates the browser ability to prevent user from leaving the page
  @HostListener('window:beforeunload', ['$event']) unloadNotification($event: any) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }
  member: Member | undefined;
  user: User | null = null;

  constructor(private accountService: AccountService, private memberService: MemberService, private toastr: ToastrService) {
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

  updateMember() {
    console.log(this.member);
    this.toastr.success('Profile has been updated successfully!');
    this.editForm?.reset(this.member); // member gonna have the updated information
  }
}
