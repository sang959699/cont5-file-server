import { Component, OnInit, Injectable } from '@angular/core';

import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { FileService } from 'src/app/services/file.service';
import { SessionService } from 'src/app/services/session.service';
import { GetStatsModel } from 'src/app/models/stats.model';
import { StatsService } from 'src/app/services/stats.service';

@Component({
  selector: 'app-login',
  templateUrl: './stats-viewer.component.html',
  styleUrls: ['./stats-viewer.component.scss']
})

@Injectable()
export class StatsViewerComponent implements OnInit {
  watchedStatsList: GetStatsModel;
  pendingStatsList: GetStatsModel;
  roleId: number;
  
  constructor(private router: Router, private sessionService: SessionService, private statsService: StatsService, private loginService: LoginService) { }

  isMobileLayout: boolean;

  ngOnInit() {
    this.isMobileLayout = window.innerWidth <= 1200;
    window.onresize = () => this.isMobileLayout = window.innerWidth <= 1200;
    
    
    this.getWatchedStats();
    this.loginService.getRoleId().subscribe(res => { 
      this.roleId = res.roleId;
      if (this.roleId <= 1) this.getPendingStats();
    });
  }
  
  getWatchedStats() {
    this.statsService.getWatchedStats().subscribe(res => {
      this.watchedStatsList = res;
    });
  }

  getPendingStats() {
    this.statsService.getPendingStats().subscribe(res => {
      this.pendingStatsList = res;
    });
  }
}
