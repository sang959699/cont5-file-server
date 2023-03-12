import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

import { LoginService } from '../../services/login.service';
import { FileService } from '../../services/file.service';
import { faSignOutAlt, faSync, faFileCode, faCloudDownloadAlt, faBookmark, faServer, faChartLine, faExclamation } from '@fortawesome/free-solid-svg-icons';
import { faYoutube } from '@fortawesome/free-brands-svg-icons';
import { SessionService } from 'src/app/services/session.service';
import { AuditService } from 'src/app/services/audit.service';
import { Router } from '@angular/router';
import { BookmarkService } from 'src/app/services/bookmark.service';
import { StatsService } from 'src/app/services/stats.service';
import { DownloaderService } from 'src/app/services/downloader.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {

  faSignOutAlt = faSignOutAlt;
  faSync = faSync;
  faFileCode = faFileCode;
  faYoutube = faYoutube;
  faCloudDownloadAlt = faCloudDownloadAlt;
  faBookmark = faBookmark;
  faServer = faServer;
  faChartLine = faChartLine;
  faExclamation = faExclamation;
  currentRoute: string;
  roleId: number;
  
  isLoggedIn$: Observable<boolean>;
  isAdminRole: boolean;

  failedDownloadFileCount: number = 0;

  constructor(private loginService: LoginService, private fileService: FileService, private sessionService: SessionService, private bookmarkService: BookmarkService, private auditService: AuditService, private router: Router, private statsService: StatsService, private downloaderService: DownloaderService) { }

  ngOnInit() {
    this.loginService.getRoleId().subscribe(res => this.roleId = res.roleId);
    this.currentRoute = this.router.url;
    this.router.events.subscribe(res => {
    this.currentRoute = this.router.url;
   });
   this.loginService.checkAdminRoleSubject.subscribe(res => {
     this.isAdminRole = res;
   });
   this.downloaderService.getFailedDownloadCountSubject().subscribe(res => {
     this.failedDownloadFileCount = res;
   })
  }

  refreshfileList() {
    if (this.router.url == '/fileList') {
      let path = this.sessionService.cont5Path;
      this.fileService.scanPath(path);
      this.fileService.updateMovableAnimePath();
    }
  }

  refreshBookmark() {
    if (this.router.url == '/bookmark') {
      this.bookmarkService.getBookmark();
    }
  }

  refreshAuditViewer() {
    if (this.router.url == '/auditViewer') {
      this.auditService.getAuditLog();
    }
  }

  refreshStatsViewer() {
    if (this.router.url == '/statsViewer') {
      this.statsService.getWatchedStats();
      if (this.roleId <= 1) this.statsService.getPendingStats();
    }
  }

  logout() {
    this.loginService.logout();
  }
}
