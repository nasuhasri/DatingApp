import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MemberService } from 'src/app/_services/member.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member | undefined;
  uploader: FileUploader | undefined;
  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User | undefined;

  constructor(private accountService: AccountService, private memberService: MemberService) {
    // sets user property
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user) this.user = user;
      }
    })
  }
  
  ngOnInit(): void {
    this.initializeUploader()
  }

  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          this.user.photoUrl = photo.url; // for navbar component
          this.accountService.setCurrentUser(this.user);
          this.member.photoUrl = photo.url; // for member component
          this.member.photos.forEach(p => {
            if (p.isMain) p.isMain = false;

            if (p.id === photo.id) p.isMain = true;
          })
        }
      }
    })
  }

  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: _ => {
        if (this.member) {
          // return all photos except photo that matches this condition
          this.member.photos = this.member.photos.filter(x => x.id !== photoId);
        }
      }
    })
  }  

  initializeUploader() {
    this.uploader = new FileUploader({
      url: `${this.baseUrl}users/add-photo`,
      authToken: `Bearer ${this.user?.token}`,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024 // 10 MB
    })

    // if we dont do this, we need to adjust cors configuration file
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false
    }

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member?.photos.push(photo);

        // check isMain() bcs when we upload photo and its user first photo, then its going to be set as isMain() by the api
        if (photo.isMain() && this.user && this.member) {
          this.user.photoUrl = photo.url;
          this.member.photoUrl = photo.url;
          this.accountService.setCurrentUser(this.user);
        }
      }
    }
  }
}
