import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { AuthenticateModel } from '../models/login.model';
import { Router } from '@angular/router';
import { BehaviorSubject, Subject, Observable, of } from 'rxjs';
import { GetRoleIdModel, RenewTokenModel, ValidateTokenModel } from 'src/app/models/login.model';
import { observeOn } from 'rxjs/operators';
import { SessionService } from './session.service';
import { GetAuditLogModel } from '../models/audit.model';

@Injectable({
  providedIn: 'root',
})
export class AuditService {
  private readonly rootUrl = window.location.protocol;
  private getAuditLogSubject:Subject<GetAuditLogModel[]> = new Subject<GetAuditLogModel[]>();
  
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

  getAuditLog(endpoint: string = '', requestResponse: string = '', createdBy: string = '') {
    let request = { endpoint, requestResponse, createdBy};
    this.http.post<GetAuditLogModel[]>((this.rootUrl + '/api/Audit/GetAuditLog'), request, this.httpOptions)
      .subscribe(res => {
        if (res) {
          this.getAuditLogSubject.next(res);
        }
      });
    return this.getAuditLogSubject;
  }
}