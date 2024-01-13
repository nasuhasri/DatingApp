import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  // $ - observable
  // members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  userParams: UserParams | undefined;
  genderList = [{value: 'male', display: 'Males'}, {value: 'female', display: 'Females'}];

  constructor(private memberService: MemberService) {
    this.userParams = this.memberService.getUserParams();
  }

  ngOnInit(): void {
    // this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  loadMembers() {
    if (this.userParams) {
      // set user params first
      this.memberService.setUserParams(this.userParams);

      this.memberService.getMembers(this.userParams).subscribe({
        next: response => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        }
      })
    }
  }

  pageChanged(event: any) {
    // check if userParams exists
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      // update the pageNumber to page number from user so the loadMembers() will auto pickup the pageNumber needed
      this.userParams.pageNumber = event.page;
      // set user params to the latest params
      this.memberService.setUserParams(this.userParams);
      this.loadMembers(); // get the updated content
    }
  }

  resetFilters() {
    // return back with default settings for filter
    this.userParams = this.memberService.resetUserParams();
    this.loadMembers();
  }
}
