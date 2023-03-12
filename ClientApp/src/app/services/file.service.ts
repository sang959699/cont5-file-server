import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { AuthenticateModel } from '../models/login.model';
import { Router } from '@angular/router';
import { BehaviorSubject, AsyncSubject, ReplaySubject, Subject } from 'rxjs';
import { ScanPathModel, GetMovablePathModel, ContFile, GenerateDownloadModel, DeleteFileModel, SubmitNoteModel, GetSubtitleModel, UploadTextFileModel, SubmitMoveModel, SubmitCreateMoveModel } from '../models/file.model';
import { LoginService } from './login.service';
import { SessionService } from './session.service';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  private scanPathSubject:Subject<ScanPathModel> = new Subject<ScanPathModel>();
  private toggleToGetMoveablePath: boolean = true;
  private getMovablePathSubject:ReplaySubject<GetMovablePathModel[]> = new ReplaySubject<GetMovablePathModel[]>(1);
  private readonly rootUrl = window.location.protocol;
  
  ngOnInit() { }

  constructor(private http: HttpClient, private loginService: LoginService, private sessionService: SessionService) { }

  get httpOptions() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.sessionService.cont5Token,
      })
    };
  }

  scanPath(path: string) {
    let request = { path };
    this.http.post<ScanPathModel>((this.rootUrl + '/api/File/ScanPath'), request, this.httpOptions).subscribe(res => {
      if (res) {
        this.sessionService.cont5Path = res.currentPath
        this.scanPathSubject.next(res);
      }
    }, () => {
      this.loginService.renewToken();
    });
    return this.scanPathSubject;
  }

  submitWatched(path: string) {
    let request = { path };
    return this.http.post<ScanPathModel>((this.rootUrl + '/api/File/SubmitWatched'), request, this.httpOptions);
  }

  getMovableAnimePath() {
    if (this.toggleToGetMoveablePath) {
      let request = { };
      this.http.post<GetMovablePathModel[]>((this.rootUrl + '/api/File/GetMovableAnimePath'), request, this.httpOptions)
        .subscribe(res => {
          if (res) {
            this.getMovablePathSubject.next(res);
          }
        });
        this.toggleToGetMoveablePath = false;
    }
    return this.getMovablePathSubject;
  }

  updateMovableAnimePath() {
    this.toggleToGetMoveablePath = true;
    this.getMovableAnimePath();
  }

  submitMoveFile(sourceFile: string, targetPath: string) {
    let request = { sourceFile, targetPath };
    return this.http.post<SubmitMoveModel>((this.rootUrl + '/api/File/SubmitMoveFile'), request, this.httpOptions);
  }

  submitCreateMoveFile(sourceFile: string, targetPath: string) {
    let request = { sourceFile, targetPath };
    return this.http.post<SubmitCreateMoveModel>((this.rootUrl + '/api/File/SubmitCreateMoveFile'), request, this.httpOptions);
  }

  generateDownload(path: string) {
    let request = { path };
    return this.http.post<GenerateDownloadModel>((this.rootUrl + '/api/File/GenerateDownload'), request, this.httpOptions);
  }

  deleteFile(path: string) {
    let request = { path };
    return this.http.post<DeleteFileModel>((this.rootUrl + '/api/File/DeleteFile'), request, this.httpOptions);
  }

  createNewFolder(path: string, name: string) {
    let request = { path, name };
    return this.http.post<DeleteFileModel>((this.rootUrl + '/api/File/CreateNewFolder'), request, this.httpOptions);
  }

  submitNote(fileName: string, note: string) {
    let request = { fileName, note };
    return this.http.post<SubmitNoteModel>((this.rootUrl + '/api/File/SubmitNote'), request, this.httpOptions);
  }

  getSubtitle(path: string) {
    let request = { path };
    return this.http.post<GetSubtitleModel>((this.rootUrl + '/api/File/GetSubtitle'), request, this.httpOptions);
  }

  downloadFile(url: string) {
    return this.http.get<string>((url));
  }

  uploadTextFile(path: string, content: string) {
    let request = { path, content };
    return this.http.post<UploadTextFileModel>((this.rootUrl + '/api/File/UploadTextFile'), request, this.httpOptions);
  }
}