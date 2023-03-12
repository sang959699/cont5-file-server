import { SafeUrl, DomSanitizer } from "@angular/platform-browser";
import { Sanitizer } from "@angular/core";

export class ScanPathModel {
    public isDirectory: boolean;
    public currentPath: string;
    public folderList: ContFolder[];
    public fileList: ContFile[];
    public isDeletable: boolean;
    public isBookmarkable: boolean;
    public isMovable: boolean;
    public movableFolderPath: GetMovablePathModel[];
    public showFolderOption: boolean;
    public isFolderCreatable: boolean;
    public windowsPlayer: string;
    public windowsUrlScheme: string;
}
export class ContFolder {
    public name: string;
    public isNew: boolean;
}
export class ContFile {
    public name: string;
    public type: number;
    public note: string;
    public isWatched: boolean;
    public isNew: boolean;
    public link: string;
    public subtitleUrl: string;
    public animeName: string;
    public fileListProp: FileListPropModel;
}
export class FileListPropModel {
    public showDownloadButton: boolean;
    public showWatchedButton: boolean;
    public showDownloadSubtitleButton: boolean;
}
export class GetMovablePathModel {
    public name: string;
    public path: string;
}
export class GenerateDownloadModel {
    public link: string;
    public generatedDt: Date;
    public expiryDt: Date;
}
export class DeleteFileModel {
    public result: boolean;
}
export class SubmitNoteModel {
    public result: boolean;
}
export class SubmitMoveModel {
    public result: boolean;
}
export class SubmitCreateMoveModel {
    public result: boolean;
}
export class GetSubtitleModel {
    public result: boolean;
    public url: string;
}
export class UploadTextFileModel {
    public result: boolean;
}
export class CustomPlayModel {
    public link: string;
    public mxLink: string;
    public windowsLink: string;
    public resumeLink: string;
    public windowsPlayer: string = "MPCBE";

    constructor(note: string, link: string, windowsPlayer: string, windowsUrlScheme: string) {
        this.link = link;
        this.mxLink = "intent:" + link + "#Intent;package=com.mxtech.videoplayer.pro;end";
        this.windowsLink = windowsUrlScheme + link;
        if (windowsPlayer) {
            this.windowsLink += ":" + windowsPlayer;
            this.windowsPlayer = windowsPlayer;
        }
        if (note) {
            let mxResumeTiming = this.getMxResumeTiming(note);
            if (mxResumeTiming) {
                this.resumeLink = "intent:"+link+"#Intent;package=com.mxtech.videoplayer.pro;i.position="+mxResumeTiming+";end";
            }
        }
    }

    getMxResumeTiming(note: string) {
        try {
            let output:string = "";
            let tempNote:string = note;
            let noteLength:number = tempNote.length;
            
            let hrs:number = 0, mins:number = 0, secs:number = 0;
            if (noteLength === 4) {
                mins = parseInt(tempNote.substr(0, 2));
                secs = parseInt(tempNote.substr(2, 2));
                output = ((mins * 60000) + (secs * 1000)).toString();
            } else if (noteLength === 6) {
                hrs = parseInt(tempNote.substr(0, 2));
                mins = parseInt(tempNote.substr(2, 2));
                secs = parseInt(tempNote.substr(4, 2));
                output = ((hrs * 3600000) + (mins * 60000) + (secs * 1000)).toString();
            } else {
                return false;
            }
            return output;
        } catch (Exception) {
            return null;
        }
    }

    public reset() {
        this.mxLink = null;
        this.resumeLink = null;
    }
}