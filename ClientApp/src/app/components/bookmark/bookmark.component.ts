import { Component, OnInit, Injectable } from '@angular/core';

import { faArrowLeft, faStar, faTimes } from '@fortawesome/free-solid-svg-icons';

import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { SessionService } from 'src/app/services/session.service';
import { FileService } from 'src/app/services/file.service';
import { GetAuditLogModel } from 'src/app/models/audit.model';
import { ContFile } from '../../models/file.model';
import { BookmarkService } from 'src/app/services/bookmark.service';
import { GetBookmarkModel } from 'src/app/models/bookmark.model';

@Component({
  selector: 'app-login',
  templateUrl: './bookmark.component.html',
  styleUrls: ['./bookmark.component.scss']
})

@Injectable()
export class BookmarkComponent implements OnInit {
  faArrowLeft = faArrowLeft;
  faStar = faStar;
  faTimes = faTimes;
  filePath: string = this.sessionService.cont5Path;
  bookmark: GetBookmarkModel[];
  
  constructor(private router: Router, private sessionService: SessionService, private bookmarkService: BookmarkService) { }

  ngOnInit() {
    this.getBookmark();
  }

  getBookmark() {
    this.bookmarkService.getBookmark().subscribe(res => {
      this.bookmark = res;
    });
  }

  deleteBookmark(path: string) {
    this.bookmarkService.deleteBookmark(path).subscribe(res => {
      if (res.result) {
        this.getBookmark();
      }
    })
  }

  goBookmark(path: string) {
    this.sessionService.cont5Path = path;
    this.goFileList();
  }

  goFileList() {
    this.router.navigate(['/fileList']);
  }
}
