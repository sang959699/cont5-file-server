import { Component, OnInit, Injectable } from '@angular/core';

import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { FileService } from 'src/app/services/file.service';
import { SessionService } from 'src/app/services/session.service';
import { AuditService } from 'src/app/services/audit.service';
import { GetAuditLogModel } from 'src/app/models/audit.model';

@Component({
  selector: 'app-login',
  templateUrl: './audit-viewer.component.html',
  styleUrls: ['./audit-viewer.component.scss']
})

@Injectable()
export class AuditViewerComponent implements OnInit {
  auditLogList: GetAuditLogModel[];
  auditLogFilterForm: UntypedFormGroup;
  
  constructor(private router: Router, private sessionService: SessionService, private auditService: AuditService, private formBuilder: UntypedFormBuilder) { }

  isMobileLayout: boolean;

  ngOnInit() {
    this.getAuditLog();
    this.auditLogFilterForm = this.formBuilder.group({
      endpoint: [''], requestResponse: [''], createdBy: ['']
    });
    
    this.isMobileLayout = window.innerWidth <= 1200;
    window.onresize = () => this.isMobileLayout = window.innerWidth <= 1200;
  }

  getAuditLog(endpoint: string = '', requestResponse: string = '', createdBy: string = '') {
    this.auditService.getAuditLog(endpoint, requestResponse, createdBy).subscribe(res => {
      this.auditLogList = res;
    });
  }
}
