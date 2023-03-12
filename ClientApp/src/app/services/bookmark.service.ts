import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { AuthenticateModel } from '../models/login.model';
import { Router } from '@angular/router';
import { BehaviorSubject, Subject, Observable, of } from 'rxjs';
import { GetRoleIdModel, RenewTokenModel, ValidateTokenModel } from 'src/app/models/login.model';
import { observeOn } from 'rxjs/operators';
import { SessionService } from './session.service';
import { AddBookmarkModel, DeleteBookmarkModel, GetBookmarkModel } from '../models/bookmark.model';

@Injectable({
  providedIn: 'root',
})
export class BookmarkService {
  private readonly rootUrl = window.location.protocol;
  
  get httpOptions() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.sessionService.cont5Token,
      })
    };
  }

  ngOnInit() { }

  constructor(private http: HttpClient, private sessionService: SessionService) { }

  private getBookmarkSubject:Subject<GetBookmarkModel[]> = new Subject<GetBookmarkModel[]>();

  addBookmark(path: string) {
    let request = { path };
    return this.http.post<AddBookmarkModel>((this.rootUrl + '/api/Bookmark/AddBookmark'), request, this.httpOptions);
  }

  deleteBookmark(path: string) {
    let request = { path };
    return this.http.post<DeleteBookmarkModel>((this.rootUrl + '/api/Bookmark/DeleteBookmark'), request, this.httpOptions);
  }

  getBookmark() {
    let request = { };
    this.http.post<GetBookmarkModel[]>((this.rootUrl + '/api/Bookmark/GetBookmark'), request, this.httpOptions)
      .subscribe(res => {
        if (res) {
          this.getBookmarkSubject.next(res);
        }
      });
    return this.getBookmarkSubject;
  }
}