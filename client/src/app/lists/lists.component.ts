import { Component, OnInit } from '@angular/core';
import { MemberService } from '../_services/member.service';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Member[] | undefined;
  predicate = 'liked';
  pageNumber = 1;
  pageSize = 5;
  pagination: Pagination | undefined;

  constructor(private memberService: MemberService) {}

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService.getLikes(this.predicate, this.pageNumber, this.pageSize).subscribe({
      next: response => {
        this.members = response.result,
        this.pagination = response.pagination
      }
    })
  }

  pageChanged(event: any) {
    // check if userParams exists
    if (this.pageNumber !== event.page) {
      // update the pageNumber to page number from user so the loadMembers() will auto pickup the pageNumber needed
      this.pageNumber = event.page;
      this.loadLikes(); // get the updated content
    }
  }
}
