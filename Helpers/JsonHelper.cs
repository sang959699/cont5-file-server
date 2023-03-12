using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Cont5.Models.Json;
using System;
using System.Linq;

namespace Cont5.Helpers
{
    public interface IJsonHelper
    {
        List<WatchedFile> WatchedList { get; }
        bool SaveWatchedList (List<WatchedFile> model);
        List<NoteFile> NoteList { get; }
        bool SaveNoteList (List<NoteFile> model);
        List<MovedFile> MovedList { get; }
        bool SaveMovedList (List<MovedFile> model);
        List<AuditLog> AuditList { get; }
        bool SaveAuditList (List<AuditLog> model);
        bool AddAuditList (AuditLog model);
        List<Bookmark> Bookmark { get; }
        bool SaveBookmark (List<Bookmark> model);
        string ToJson (dynamic model);
        bool IsValidJsonString (string json);
        AuditLog GetLatestAuditLog();
        List<FailedDownloadedFile> FailedDownloadFileList { get; }
    }
    public class JsonHelper : IJsonHelper
    {
        private readonly IConfigHelper configHelper;
        private readonly IUserAccountHelper userAccountHelper;
        public JsonHelper(IConfigHelper configHelper,IUserAccountHelper userAccountHelper) {
            this.configHelper = configHelper;
            this.userAccountHelper = userAccountHelper;
        }
        
        #region WatchedList
        public List<WatchedFile> WatchedList {
            get {
                var watchPath = configHelper.WatchedJsonPath;
                var json = File.ReadAllText(watchPath);

                var result = JsonConvert.DeserializeObject<List<WatchedFile>>(json);
                if (result == null)  return new List<WatchedFile>();

                return result;
            }
        }
        
    
        public bool SaveWatchedList(List<WatchedFile> model) {
            var watchPath = configHelper.WatchedJsonPath;
            var json = JsonConvert.SerializeObject(model);

            File.WriteAllText(watchPath, json);
            return true;
        }
        #endregion

        #region NoteList
        public List<NoteFile> NoteList {
            get {
                var notePath = configHelper.NoteJsonPath;
                var json = File.ReadAllText(notePath);

                var result = JsonConvert.DeserializeObject<List<NoteFile>>(json);
                if (result == null)  return new List<NoteFile>();

                return result;
            }
        }
        public bool SaveNoteList(List<NoteFile> model) {
            var notePath = configHelper.NoteJsonPath;
            var json = JsonConvert.SerializeObject(model);

            File.WriteAllText(notePath, json);
            return true;
        }
        #endregion
    
        #region MovedList
        public List<MovedFile> MovedList {
            get {
                var movedPath = configHelper.MovedJsonPath;
                var json = File.ReadAllText(movedPath);

                var result = JsonConvert.DeserializeObject<List<MovedFile>>(json);
                if (result == null)  return new List<MovedFile>();

                return result;
            }
        }
        public bool SaveMovedList(List<MovedFile> model) {
            var movedPath = configHelper.MovedJsonPath;
            var json = JsonConvert.SerializeObject(model);

            File.WriteAllText(movedPath, json);
            return true;
        }
        #endregion
    
        #region AuditList
        public List<AuditLog> AuditList {
            get {
                var auditPath = configHelper.AuditJsonPath;
                var json = File.ReadAllText(auditPath);

                var result = JsonConvert.DeserializeObject<List<AuditLog>>(json);
                if (result == null)  return new List<AuditLog>();

                return result;
            }
        }

        public AuditLog GetLatestAuditLog() {
            var result = new AuditLog(); 
            if (AuditList.Count > 0) {
                result = AuditList.OrderByDescending(o => o.CreatedDt).FirstOrDefault();
            }
            return result;
        }
        public bool SaveAuditList (List<AuditLog> model) {
            var auditPath = configHelper.AuditJsonPath;
            var json = JsonConvert.SerializeObject(model);

            File.WriteAllText(auditPath, json);
            return true;
        }

        public bool AddAuditList(AuditLog model) {
            try {
                var auditPath = configHelper.AuditJsonPath;
                var json = File.ReadAllText(auditPath);
                var auditLogList = JsonConvert.DeserializeObject<List<AuditLog>>(json);
                
                auditLogList ??= new List<AuditLog>();
                auditLogList.Add(model);
                json = JsonConvert.SerializeObject(auditLogList);

                File.WriteAllText(auditPath, json);
                return true;
            } catch (Exception) {
                return false;
            }
        }
        #endregion

        #region Bookmark
        public List<Bookmark> Bookmark {
            get {
                var bookmarkPath = configHelper.BookmarkJsonPath;
                var json = File.ReadAllText(bookmarkPath);

                var result = JsonConvert.DeserializeObject<List<Bookmark>>(json);
                if (result == null)  return new List<Bookmark>();

                return result;
            }
        }

        public bool SaveBookmark (List<Bookmark> model) {
            var bookmarkPath = configHelper.BookmarkJsonPath;
            var json = JsonConvert.SerializeObject(model);

            File.WriteAllText(bookmarkPath, json);
            return true;
        }
        #endregion

        #region FailedDownloadFileList
        public List<FailedDownloadedFile> FailedDownloadFileList {
            get {
                var failedDownloadFilePath = configHelper.FailedDownloadFileJsonPath;
                var json = File.ReadAllText(failedDownloadFilePath);

                var result = JsonConvert.DeserializeObject<List<FailedDownloadedFile>>(json);
                if (result == null)  return new List<FailedDownloadedFile>();

                return result;
            }
        }
        #endregion

        public string ToJson (dynamic model) {
            return JsonConvert.SerializeObject(model);
        }
        public bool IsValidJsonString(string json) {
            var result = true;
            try {
                JsonConvert.DeserializeObject(json);
            } catch (Exception) {
                result = false;
            }
            return result;
        }
    }
}