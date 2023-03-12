import { Component, OnInit, SecurityContext } from '@angular/core';
import { DownloaderService } from 'src/app/services/downloader.service';
import { GetAriaNgUrlModel } from 'src/app/models/downloader.model';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-aria-ng',
  templateUrl: './aria-ng.component.html',
  styleUrls: ['./aria-ng.component.scss']
})
export class AriaNgComponent implements OnInit {

  constructor(private downloaderService: DownloaderService, private sanitizer: DomSanitizer) { }

  ariaNgUrl: SafeUrl;

  ngOnInit(): void {
    this.getAriaNgUrl();
    this.downloaderService.getFailedDownloadCountSubject().pipe(first()).subscribe();
  }

  getAriaNgUrl() {
    this.downloaderService.getAriaNgUrl().subscribe(res => {
      this.ariaNgUrl = this.sanitizeUrl(res.url);
    });
  }

  sanitizeUrl(url:string){
    return this.sanitizer.bypassSecurityTrustResourceUrl(url);
  }
}
