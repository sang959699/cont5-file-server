using System;

namespace Cont5.Models.Json {
    public class WatchedFile {
        public string FileName { get; set; }
        public bool IsWatched { get; set; }
        public long Duration { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }
    public class NoteFile {
        public string FileName { get; set; }
        public string Note { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }

    public class MovedFile {
        public string FileName { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public bool Status { get; set; }
        public dynamic Exception { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }
    public class AuditLog {
        public string Api { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Exception { get; set; }
        public string ClientIp { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
    }
    public class Bookmark {
        public string Path { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UpdatedDt { get; set; }
    }
    public class FailedDownloadedFile{
        public string FileName { get; set; }
        public string MagnetLink { get; set; }
        public DateTime AttemptDT { get; set; }
    }
}