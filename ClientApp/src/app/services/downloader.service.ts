import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { AuthenticateModel } from '../models/login.model';
import { Router } from '@angular/router';
import { BehaviorSubject, Subject, Observable, of } from 'rxjs';
import { GetRoleIdModel, RenewTokenModel, ValidateTokenModel } from 'src/app/models/login.model';
import { observeOn } from 'rxjs/operators';
import { SessionService } from './session.service';
import { GetAriaNgUrlModel, GetFailedDownloadCountModel } from '../models/downloader.model';

@Injectable({
  providedIn: 'root',
})
export class DownloaderService {
  private readonly rootUrl = window.location.protocol;
  readonly failedDownloadCount:Subject<number> = new Subject<number>();
  
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

  getAriaNgUrl() {
    let request = {};
    return this.http.post<GetAriaNgUrlModel>((this.rootUrl + '/api/Downloader/GetAriaNgUrl'), request, this.httpOptions);
  }
  getFailedDownloadCount() {
    let request = {};
    return this.http.post<GetFailedDownloadCountModel>((this.rootUrl + '/api/Downloader/GetFailedDownloadCount'), request, this.httpOptions);
  }

  getFailedDownloadCountSubject() {
    let request = { };
    this.http.post<GetFailedDownloadCountModel>((this.rootUrl + '/api/Downloader/GetFailedDownloadCount'), request, this.httpOptions).subscribe(res => {
      this.failedDownloadCount.next(res.result);
    });
    return this.failedDownloadCount;
  }
}