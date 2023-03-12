import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { AuthenticateModel } from '../models/login.model';
import { Router } from '@angular/router';
import { BehaviorSubject, Subject, Observable, of } from 'rxjs';
import { GetRoleIdModel, RenewTokenModel, ValidateTokenModel } from 'src/app/models/login.model';
import { observeOn } from 'rxjs/operators';
import { SessionService } from './session.service';
import { GetAuditLogModel } from '../models/audit.model';
import { GetStatsModel } from '../models/stats.model';

@Injectable({
  providedIn: 'root',
})
export class StatsService {
  private readonly rootUrl = window.location.protocol;
  private getWatchedStatsSubject:Subject<GetStatsModel> = new Subject<GetStatsModel>();
  private getPendingStatsSubject:Subject<GetStatsModel> = new Subject<GetStatsModel>();
  
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

  getWatchedStats() {
    let request = { };
    this.http.post<GetStatsModel>((this.rootUrl + '/api/Stats/GetWatchedStats'), request, this.httpOptions)
      .subscribe(res => {
        if (res) {
          this.getWatchedStatsSubject.next(res);
        }
      });
    return this.getWatchedStatsSubject;
  }

  getPendingStats() {
    let request = { };
    this.http.post<GetStatsModel>((this.rootUrl + '/api/Stats/GetPendingStats'), request, this.httpOptions)
      .subscribe(res => {
        if (res) {
          this.getPendingStatsSubject.next(res);
        }
      });
    return this.getPendingStatsSubject;
  }
}