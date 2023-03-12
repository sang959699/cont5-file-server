import { Component, OnInit, Injectable } from '@angular/core';
import { FileService } from '../../services/file.service';
import { LoginService } from '../../services/login.service';
import { faHome, faChevronRight, faFolder, faClock, faFile, faArrowLeft, faCheck, faTimes, faLink, faPlay, faCaretDown, faFlag, faDownload, faTrash, faCode, faPen, faArchive } from '@fortawesome/free-solid-svg-icons';
import { faYoutube } from '@fortawesome/free-brands-svg-icons';
import { ScanPathModel, CustomPlayModel } from 'src/app/models/file.model';
import { ContFile, ContFolder, GetMovablePathModel } from '../../models/file.model';
import { ReplaySubject, Subscription } from 'rxjs';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { SessionService } from 'src/app/services/session.service';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { BookmarkService } from 'src/app/services/bookmark.service';

@Component({
  selector: 'app-file-list',
  templateUrl: './file-list.component.html',
  styleUrls: ['./file-list.component.scss'],
  animations: [
    trigger('hideByVertical', [
      state('closed', style({
        opacity: '0',
        height: '0',
        pointerEvents: 'none',
        fontSize: '0'
      })),
      state('open', style({
        marginBottom: '.5rem'
      })),
      transition('closed <=> open', animate(100)),
    ]),
    trigger('hideByHorizontal', [
      state('closed', style({
        opacity: '0',
        width: '0',
        margin: '0',
        padding: '0',
        pointerEvents: 'none',
        fontSize: '0'
      })),
      state('open', style({
      })),
      transition('closed <=> open', animate(100)),
    ])
  ]
})

@Injectable()
export class FileListComponent implements OnInit {
  isMobile = (() => (typeof window.orientation !== "undefined") || (navigator.userAgent.indexOf('IEMobile') !== -1));
  isIos =  (() => [
    'iPad Simulator',
    'iPhone Simulator',
    'iPod Simulator',
    'iPad',
    'iPhone',
    'iPod'
  ].includes(navigator.platform)
  // iPad on iOS 13 detection
  || (navigator.userAgent.includes("Mac") && "ontouchend" in document));

  faHome = faHome;
  faChevronRight = faChevronRight;
  faFolder = faFolder;
  faClock = faClock;
  faYoutube = faYoutube;
  faFile = faFile;
  faArrowLeft = faArrowLeft;
  faCheck = faCheck;
  faTimes = faTimes;
  faLink = faLink;
  faPlay = faPlay;
  faCaretDown = faCaretDown;
  faFlag = faFlag;
  faDownload = faDownload;
  faTrash = faTrash;
  faCode = faCode;
  faPen = faPen;
  faArchive = faArchive;

  filePath: string = this.sessionService.cont5Path;
  pathList: string[] = (this.filePath === "" ? [] : this.filePath.split('/'));
  scanPathModel: ScanPathModel = new ScanPathModel();
  roleId: number;
  isLoad: boolean;
  fileDetails: ContFile;
  animeList: ContFolder[];
  movableAnimePath: GetMovablePathModel[];
  movePathSubject: ReplaySubject<string> = new ReplaySubject<string>(1);
  moveFileForm: UntypedFormGroup;
  createMoveFileForm: UntypedFormGroup;
  createFolderForm: UntypedFormGroup;
  moveFolderForm: UntypedFormGroup;
  submitNoteForm: UntypedFormGroup;
  countDeleteSubmitted: number;
  isMoveSubmitted: boolean;
  isMoveFromFileListIndex: number;
  countBookmarkSubmitted: number;
  showVideoOnly: boolean = false;
  showFolderOption: boolean = false;
  isCreateSubmitMode: boolean = false;

  isFolderOptionHide: boolean = true;
  isCreateFolderHide: boolean = true;
  folderNameValue: string;

  customPlayModel: CustomPlayModel;

  scanPathSubscription: Subscription;

  constructor(private fileService: FileService, private loginService: LoginService, private formBuilder: UntypedFormBuilder, private sessionService: SessionService, private bookmarkService: BookmarkService) { }
  
  ngOnDestroy() {
    this.scanPathModel = new ScanPathModel();
    this.scanPathSubscription.unsubscribe();
  }

  ngOnInit() {
    this.scanPath(this.filePath);
    this.loginService.getRoleId().subscribe(res => this.roleId = res.roleId);
    this.fileService.getMovableAnimePath().subscribe(res => this.movableAnimePath = res);
    this.moveFileForm = this.formBuilder.group({
      movePath: ['', Validators.required]
    });
    this.createMoveFileForm = this.formBuilder.group({
      createMovePath: ['', Validators.required]
    });
    this.createFolderForm = this.formBuilder.group({
      folderName: ['', Validators.required]
    });
    this.moveFolderForm = this.formBuilder.group({
      movePath: ['', Validators.required]
    });
    this.submitNoteForm = this.formBuilder.group({
      note: ['', Validators.pattern(/[0-9]*/)]
    });
    this.rerenderFolderOption();
  }

  rerenderFolderOption = () => {
    this.showVideoOnly = false;
    this.showFolderOption = false;
  };

  toggleFolderOption() {
    if (this.scanPathModel.isDirectory) {
      this.isFolderOptionHide = !this.isFolderOptionHide;
    }
  }

  resetFolderOption() {
    this.isFolderOptionHide = true;
    this.isCreateFolderHide = true;
    this.createFolderForm.reset();
    this.moveFolderForm.reset();
    this.createMoveFileForm.reset();
    this.countBookmarkSubmitted = 0;
    this.countDeleteSubmitted = 0;
    this.isCreateSubmitMode = false;
  }

  backPath(index: number) {
    let newPath: string = this.pathList.filter((u, i) => i < (index + 1)).join('/');
    this.rerenderFolderOption();
    this.fileService.scanPath(newPath).pipe(first()).subscribe(res => {
      window.scroll(0, 0);
    });
  }

  nextPath(name: string) {
    let newPath: string = this.scanPathModel.currentPath + '/' + name;
    this.rerenderFolderOption();
    this.fileService.scanPath(newPath).pipe(first()).subscribe(res => {
      window.scroll(0, 0);
    });
  }

  scanPath(path: string) {
    this.scanPathSubscription = this.fileService.scanPath(path).subscribe(res => {
      this.isMoveSubmitted = false;
      this.countDeleteSubmitted = 0;
      if (res.isDirectory) {
        this.fileDetails = null;
      } else {
        this.fileDetails = res.fileList[0];
        this.moveFileForm.reset();
        this.submitNoteForm.reset();
        if (this.fileDetails.type == 1) {
          this.customPlayModel = new CustomPlayModel(this.fileDetails.note, this.fileDetails.link, res.windowsPlayer, res.windowsUrlScheme);
          this.fileService.getMovableAnimePath().subscribe(res => {
            let animePathItem = this.movableAnimePath.filter(f => f.name == this.fileDetails.animeName)[0];
            if (animePathItem) {
              this.moveFileForm.patchValue({ movePath: animePathItem.path });
              this.pushMovePath();
            }
          });
        }
      }
      this.scanPathModel = res;
      this.filePath = res.currentPath;
      this.pathList = (this.filePath == '' ? [] : this.filePath.split('/'));
      this.isLoad = true;
      this.showFolderOption = this.scanPathModel.showFolderOption;

      if (this.showVideoOnly) {
        this.scanPathModel.folderList = [];
        this.scanPathModel.fileList = this.scanPathModel.fileList.filter(f => f.type == 1);
      }
      
      this.pushMovePath();
      this.resetFolderOption();
      this.isMoveFromFileListIndex = null;
    });
  }

  submitWatched(fileName?: string) {
    this.fileService.submitWatched(fileName ? this.filePath + "/" + fileName : this.filePath).subscribe(res => {
      this.fileService.scanPath(this.filePath);
    })
  }

  pushMovePath() {
    this.movePathSubject.next(this.moveFileForm.value.movePath);
  }

  submitMove(sourceFile: string, targetPath: string) {
    this.isMoveSubmitted = true;
    this.fileService.submitMoveFile(sourceFile, targetPath).subscribe(res => {
      if (this.scanPathModel.isDirectory) {
        this.fileService.updateMovableAnimePath();
      }
      this.fileService.scanPath(this.filePath);
    })
  }

  submitCreateMove(sourceFile: string, targetPath: string) {
    this.isMoveSubmitted = true;
    this.fileService.submitCreateMoveFile(sourceFile, targetPath).subscribe(res => {
      if (this.scanPathModel.isDirectory) {
        this.fileService.updateMovableAnimePath();
      }
      this.fileService.scanPath(this.filePath);
    })
  }

  submitMoveFromFileList(sourceFile: string, targetPath: string, fileListIndex: number) {
    if (this.isMoveFromFileListIndex != fileListIndex) {
      this.isMoveFromFileListIndex = fileListIndex;
      return;
    }
    targetPath = this.movableAnimePath.filter(f => f.name == targetPath)[0].path;
    this.fileService.submitMoveFile(this.scanPathModel.currentPath + '/' + sourceFile, targetPath).subscribe(res => {
      if (this.scanPathModel.isDirectory) {
        this.fileService.updateMovableAnimePath();
      }
      this.fileService.scanPath(this.filePath);
    });
  }

  // 1: download, 2: mx player, 3: mpc-be
  downloadFile = (file: ContFile, openWith: number) => {
    let path = this.scanPathModel.isDirectory ? this.scanPathModel.currentPath + '/' + file.name : this.scanPathModel.currentPath;
    this.fileService.generateDownload(path).subscribe(res => {
      let link: string = '';
      let customPlayModel = new CustomPlayModel(null, res.link, this.scanPathModel.windowsPlayer, this.scanPathModel.windowsUrlScheme);
      switch (openWith) {
        case 1: link = customPlayModel.link; break;
        case 2: link = customPlayModel.mxLink; break;
        case 3: link = customPlayModel.windowsLink; break;
      }
      window.location.href = link;
    });
  }

  downloadSubtitle = (file: ContFile) => {
    let path = this.scanPathModel.isDirectory ? this.scanPathModel.currentPath + '/' + file.name : this.scanPathModel.currentPath;
    this.fileService.getSubtitle(path).subscribe(res => {
      if (res.result) {
        window.location.href = res.url;
      }
    });
  }

  redirect = (link: string) => {
    window.location.href = link;
  }

  deleteFile = (path: string) => {
    if (this.countDeleteSubmitted == 0) {
      this.countDeleteSubmitted = 1;
      return;
    }
    this.countDeleteSubmitted = 2;
    this.fileService.deleteFile(path).subscribe(res => {
      this.fileService.updateMovableAnimePath();
      this.fileService.scanPath(this.filePath).pipe(first());
    });
  }
  
  createNewFolder = () => {
    this.fileService.createNewFolder(this.scanPathModel.currentPath, this.createFolderForm.value.folderName).subscribe(res => {
      this.fileService.updateMovableAnimePath();
      this.fileService.scanPath(this.filePath).pipe(first());
    });
  }

  submitNote = (note: string) => {
    this.fileService.submitNote(this.fileDetails.name, note).subscribe(res => {
      this.fileService.scanPath(this.filePath).pipe(first());
    });
  }

  addBookmark = (path: string) => {
    if (this.countBookmarkSubmitted == 0) {
      this.countBookmarkSubmitted = 1;
      return;
    }
    this.countBookmarkSubmitted = 2;
    this.bookmarkService.addBookmark(path).subscribe(res => {
      this.fileService.scanPath(this.filePath).pipe(first());
    });
  }

  toggleVideoOnly = () => {
    this.showVideoOnly = !this.showVideoOnly;
    this.fileService.scanPath(this.filePath).pipe(first());
  }
}