import { Component, OnInit, Injectable } from '@angular/core';

import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';

import { LoginService } from '../../services/login.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable, ReplaySubject, Subscription } from 'rxjs';
import { first } from 'rxjs/operators';
import { SessionService } from 'src/app/services/session.service';
import { FileService } from 'src/app/services/file.service';
import { GetAuditLogModel } from 'src/app/models/audit.model';
import { ContFile } from '../../models/file.model';

@Component({
  selector: 'app-login',
  templateUrl: './text-editor.component.html',
  styleUrls: ['./text-editor.component.scss']
})

@Injectable()
export class TextEditorComponent implements OnInit {
  faArrowLeft = faArrowLeft;
  filePath: string = this.sessionService.cont5Path;
  fileDetails: ContFile;
  fileContent: string;
  fileContentSubject: ReplaySubject<string> = new ReplaySubject<string>(1);
  saveStatus: boolean;
  
  constructor(private router: Router, private sessionService: SessionService, private fileService: FileService) { }

  ngOnInit() {
    this.scanPath(this.filePath);
  }

  scanPath(path: string) {
    this.fileService.scanPath(path).pipe(first()).subscribe(res => {
      if (res.isDirectory) {
        this.router.navigate(['/fileList']);
      } else {
        this.fileDetails = res.fileList[0];
        if (this.fileDetails.type != 2) {
          this.router.navigate(['/fileList']);
        }
        this.downloadFile();
      }
    });
  }

  pushFileContent() {
    this.fileContentSubject.next(this.fileContent);
  }

  downloadFile() {
    this.fileService.downloadFile(this.fileDetails?.link).subscribe(res => {
      this.fileContent = JSON.stringify(res, undefined, 4);
      this.fileContentSubject.next(this.fileContent);
    })
  }

  beautify() {
    this.fileContent = JSON.stringify(JSON.parse(this.fileContent), undefined, 4);
  }

  saveFile() {
    try {
      var content = JSON.stringify(JSON.parse(this.fileContent))
    } catch (ex) {
      this.saveStatus = false;
      return false;
    }
    this.fileService.uploadTextFile(this.filePath, content).pipe(first()).subscribe(res => {
      this.saveStatus = res.result;
      this.scanPath(this.filePath);
    });
  }
}
