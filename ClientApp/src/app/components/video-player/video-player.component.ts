import { Component, OnInit, Injectable } from '@angular/core';

import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';

import { Router } from '@angular/router';
import { first } from 'rxjs/operators';
import { SessionService } from 'src/app/services/session.service';
import { FileService } from 'src/app/services/file.service';
import { ContFile } from '../../models/file.model';

@Component({
  selector: 'app-login',
  templateUrl: './video-player.component.html',
  styleUrls: ['./video-player.component.scss']
})

@Injectable()
export class VideoPlayerComponent implements OnInit {
  faArrowLeft = faArrowLeft;
  filePath: string = this.sessionService.cont5Path;
  fileDetails: ContFile;
  
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
        if (this.fileDetails.type != 1) {
          this.router.navigate(['/fileList']);
        }
      }
    });
  }
}
