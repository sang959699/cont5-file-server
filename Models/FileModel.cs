using System.Collections.Generic;
using System;

namespace Cont5.Models.File {
    public class GenerateDownloadRequest {
        public string Path { get; set; }
        public GenerateDownloadRequest(string path){
            this.Path = path;
        }
    }
    public class GenerateDownloadModel {
        public string Link { get; set; }
        public DateTime GeneratedDt { get; set; }
        public DateTime ExpiryDt { get; set; }
    }
    public class GetMovablePathModel {
        public string Name { get; set; }
        public string Path { get; set; }
        public GetMovablePathModel (string Name, string Path) {
            this.Name = Name;
            this.Path = Path;
        }
    }
    public class SanitizedHiddenPathModel{
        public bool IsPathChanged { get; set; }
        public string Path { get; set; }
    }
    public class DeleteFileRequest { 
        public string Path { get; set; }
    }

    public class DeleteFileModel {
        public bool Result { get; set; }
    }
    public class CreateNewFolderRequest { 
        public string Path { get; set; }
        public string Name { get; set; }
    }

    public class CreateNewFolderModel {
        public bool Result { get; set; }
    }
    public class GetJustMovedModel {
        public List<File> FileList { get; set; }
        public class File {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }
    public class SubmitMoveFileRequest { 
        public string SourceFile { get; set; }
        public string TargetPath { get; set; }
    }

    public class SubmitMoveFileModel {
        public bool Result { get; set; }
    }
    public class SubmitCreateMoveFileRequest { 
        public string SourceFile { get; set; }
        public string TargetPath { get; set; }
    }

    public class SubmitCreateMoveFileModel {
        public bool Result { get; set; }
    }
    public class DownloadFileRequest {
        public string Path { get; set; }
        public string Token { get; set; }
    }
    public class DownloadFileModel {
        public SanitizePathModel SanitizedPath { get; set; }
    }
    public class ScanPathRequest {
        public string Path { get; set; }
    }
    public class ScanPathModel {
        public bool IsDirectory { get; set; }
        public string CurrentPath { get; set; }
        public bool ShowFolderOption { get; set; }
        public bool IsDeletable { get; set; }
        public bool IsBookmarkable { get; set; }
        public bool IsFolderCreatable { get; set; }
        public bool isMovable { get; set; }
        public List<GetMovablePathModel> MovableFolderPath { get; set; }
        public List<ContFolder> FolderList { get; set; }
        public List<ContFile> FileList { get; set; }
        public string WindowsPlayer { get; set; }
        public string WindowsUrlScheme { get; set; }
    }

    public class SubmitWatchedRequest { 
        public string Path { get; set; }
    }

    public class SubmitWatchedModel {
        public bool Result { get; set; }
    }

    public class SubmitNoteRequest { 
        public string FileName { get; set; }
        public string Note { get; set; }
    }

    public class SubmitNoteModel {
        public bool Result { get; set; }
    }

    public class SanitizePathModel {
        public string Path { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }
        public string RelativePath { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsPathChanged { get; set; }
        public bool IsVideo { get; set; }
        public bool IsInAnimeDirectory { get; set; }
        public bool IsInAnimeEnd { get; set; }
    }

    public class ContFolder {
        public string Name { get; set; }
        public bool IsNew { get; set; }
    }

    public class ContFile {
        public string Name { get; set; }
        public int Type { get; set; }
        public string Note { get; set; }
        public bool IsWatched { get; set; }
        public bool IsNew { get; set; }
        public string Link { get; set; }
        public string SubtitleUrl { get; set; }
        public string AnimeName { get; set; }
        public FileListPropModel fileListProp { get; set; }
    }
    
    public class FileListPropModel {
        public bool ShowDownloadButton { get; set; }
        public bool ShowDownloadSubtitleButton { get; set; }
        public bool ShowWatchedButton { get; set; }
    }

    public class GetSubtitleRequest {
        public string Path { get; set; }
    }

    public class GetSubtitleModel {
        public bool Result { get; set; }
        public string Url { get; set; }
    }

    public class UploadTextFileRequest {
        public string Path { get; set; }
        public string Content { get; set; }
    }

    public class UploadTextFileModel {
        public bool Result { get; set; }
    }
}