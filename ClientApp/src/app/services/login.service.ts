import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { AuthenticateModel } from '../models/login.model';
import { Router } from '@angular/router';
import { BehaviorSubject, Subject, Observable, of, ReplaySubject } from 'rxjs';
import { GetRoleIdModel, RenewTokenModel, ValidateTokenModel } from 'src/app/models/login.model';
import { observeOn } from 'rxjs/operators';
import { SessionService } from './session.service';

@Injectable({
  providedIn: 'root',
})
export class LoginService {
  isLoggedIn = (() => (this.sessionService.cont5Token != null));

  private readonly authenticateSubject:Subject<AuthenticateModel> = new Subject<AuthenticateModel>();
  private readonly getRoleIdSubject:ReplaySubject<GetRoleIdModel> = new ReplaySubject<GetRoleIdModel>(1);
  private toggleToGetRoleId: boolean = true;
  readonly checkAdminRoleSubject:ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  readonly isTokenValidateSubject:Subject<boolean> = new Subject<boolean>();
  private readonly rootUrl = window.location.protocol;
  roleId: number = 0;
  
  get httpOptions() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.sessionService.cont5Token,
      })
    };
  }

  ngOnInit() { }

  constructor(private http: HttpClient, private router: Router, private sessionService: SessionService) { }

  authenticate(userName: string, password: string) {
    let request = { userName, password };
    this.http.post<AuthenticateModel>((this.rootUrl + '/api/Login/Authenticate'), request)
      .subscribe(res => {
        if (res.token) {
          this.sessionService.cont5Token = res.token;
          this.router.navigate(['/fileList']);
        }
        this.authenticateSubject.next(res);
        this.updateRoleId();
      });
      
    return this.authenticateSubject;
  }

  checkAdminRole() {
    this.getRoleId().subscribe();
    return this.checkAdminRoleSubject;
  }

  getRoleId() {
    if (this.toggleToGetRoleId) {
      let request = { };
      this.http.post<GetRoleIdModel>((this.rootUrl + '/api/Login/GetRoleId'), request, this.httpOptions)
        .subscribe(res => {
          if (res) {
            this.getRoleIdSubject.next(res);
            this.checkAdminRoleSubject.next(res.roleId == 1);
            this.roleId = res.roleId;
          }
        });
        this.toggleToGetRoleId = false;
    }
    
    return this.getRoleIdSubject;
  }

  updateRoleId() {
    this.toggleToGetRoleId = true;
    this.getRoleId();
  }

  logout() {
    this.sessionService.clear();
    this.getRoleIdSubject.next(new GetRoleIdModel);
    this.checkAdminRoleSubject.next(false);
    this.roleId = 0;
    this.isTokenValidateSubject.next(false);
    this.router.navigate(['/login']);
  }

  renewToken() {
    let request = { };
    this.http.post<RenewTokenModel>((this.rootUrl + '/api/Login/RenewToken'), request, this.httpOptions).subscribe(res => {
      if (res) {
        this.sessionService.cont5Token = res.token;
      }
    }, () => {
      this.logout();
    });
  }

  validateToken() {
    let token = this.sessionService.cont5Token;
    let request = { token };
    this.http.post<ValidateTokenModel>((this.rootUrl + '/api/Login/ValidateToken'), request).subscribe(res => {
      this.isTokenValidateSubject.next(res.result);
      if (!res.result) {
        this.logout();
      }
    });
    this.getRoleId();
    return this.isTokenValidateSubject;
  }
}