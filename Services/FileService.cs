using Cont5.Models.File;
using Cont5.Helpers;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using Cont5.Models.Json;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Cont5.Services {
    public interface IFileService {
        DownloadFileModel DownloadFile(DownloadFileRequest request);
        ScanPathModel ScanPath(ScanPathRequest request);
        SubmitWatchedModel SubmitWatched(SubmitWatchedRequest request);
        SubmitNoteModel SubmitNote(SubmitNoteRequest request);
        SubmitMoveFileModel SubmitMoveFile (SubmitMoveFileRequest request);
        SubmitCreateMoveFileModel SubmitCreateMoveFile (SubmitCreateMoveFileRequest request);
        GetJustMovedModel GetJustMoved ();
        CreateNewFolderModel CreateNewFolder (CreateNewFolderRequest request);
        DeleteFileModel DeleteFile (DeleteFileRequest request);
        GenerateDownloadModel GenerateDownload(GenerateDownloadRequest model, SanitizePathModel sanitizedPath = null);
        List<GetMovablePathModel> GetMovableAnimePath ();
        SanitizePathModel SanitizePath(string path, bool isAbsolutePath);
        SanitizedHiddenPathModel SanitizeHiddenPath (string path);
        GetSubtitleModel GetSubtitle(string path);
        UploadTextFileModel UploadTextFile(UploadTextFileRequest request);
    }

    public class FileService : IFileService {
        private readonly IConfigHelper configHelper;
        private readonly IJsonHelper jsonHelper;
        private readonly IUserAccountHelper userAccountHelper;
        private readonly IEncryptionHelper encryptionHelper;
        private readonly string rootPath;
        private readonly string rootUrl;

        public FileService(IConfigHelper configHelper, IJsonHelper jsonHelper, IUserAccountHelper userAccountHelper, IEncryptionHelper encryptionHelper) {
            this.userAccountHelper = userAccountHelper;
            this.encryptionHelper = encryptionHelper;
            this.configHelper = configHelper;
            this.jsonHelper = jsonHelper;
            rootPath = configHelper.RootPath;
            rootUrl = configHelper.RootUrl;
        }
        
        public SubmitWatchedModel SubmitWatched(SubmitWatchedRequest request) {
            var result = new SubmitWatchedModel();

            if (request == null || String.IsNullOrWhiteSpace(request.Path)) return result;

            var sanitizedItem = SanitizePath(request.Path, false);
            result.Result = SimpleSubmitWatched(sanitizedItem, false);

            jsonHelper.AddAuditList(new AuditLog {  
                Api = MethodBase.GetCurrentMethod().Name,
                Request = jsonHelper.ToJson(request),
                Response = jsonHelper.ToJson(result),
                Exception = null,
                ClientIp = userAccountHelper.GetClientIp(),
                CreatedBy = userAccountHelper.CurrentLogin.UserId,
                CreatedDt = DateTime.Now,
            });

            return result;
        }

        private bool SimpleSubmitWatched(SanitizePathModel sanitizeItem, bool alwaysWatchedMode) {
            var result = false;
            var watchedList = jsonHelper.WatchedList;
            var watchedFile = watchedList.Where(w => w.FileName == sanitizeItem.Name && w.CreatedBy == userAccountHelper.CurrentLogin.UserId).FirstOrDefault();
            
            if (watchedFile == null) {
                double duration = 0;
                try {
                    using var mediaInfo = new MediaInfo.MediaInfo();
                    mediaInfo.Open(sanitizeItem.Path);
                    duration = Convert.ToDouble(mediaInfo.Get(MediaInfo.StreamKind.Video, 0, "Duration"));
                } catch (Exception ex) { 
                    jsonHelper.AddAuditList(new AuditLog {  
                        Api = MethodBase.GetCurrentMethod().Name,
                        Request = jsonHelper.ToJson(sanitizeItem),
                        Exception = ex.Message,
                        ClientIp = userAccountHelper.GetClientIp(),
                        CreatedBy = userAccountHelper.CurrentLogin.UserId,
                        CreatedDt = DateTime.Now,
                    });
                }
                watchedList.Add(new WatchedFile{
                    FileName = sanitizeItem.Name,
                    Duration = (long)duration,
                    CreatedBy = userAccountHelper.CurrentLogin.UserId,
                    CreatedDt = DateTime.Now,
                    IsWatched = true,
                });
            } else watchedFile.IsWatched = alwaysWatchedMode || !watchedFile.IsWatched;

            result = jsonHelper.SaveWatchedList(watchedList);

            return result;
        }

        public SubmitNoteModel SubmitNote(SubmitNoteRequest request) {
            var result = new SubmitNoteModel();

            if (request == null || String.IsNullOrWhiteSpace(request.FileName)) return result;

            var noteList = jsonHelper.NoteList;
            var notedFile = noteList.Where(w => w.FileName == request.FileName && w.CreatedBy == userAccountHelper.CurrentLogin.UserId).FirstOrDefault();
            var note = String.IsNullOrWhiteSpace(request.Note) ? "" : request.Note;

            if (notedFile == null) {
                noteList.Add(new NoteFile{
                    FileName = request.FileName,
                    Note = note,
                    CreatedBy = userAccountHelper.CurrentLogin.UserId,
                    CreatedDt = DateTime.Now,
                });
            } else {
                notedFile.Note = note;
            }
            result.Result = jsonHelper.SaveNoteList(noteList);

            jsonHelper.AddAuditList(new AuditLog {  
                Api = MethodBase.GetCurrentMethod().Name,
                Request = jsonHelper.ToJson(request),
                Response = jsonHelper.ToJson(result),
                Exception = null,
                ClientIp = userAccountHelper.GetClientIp(),
                CreatedBy = userAccountHelper.CurrentLogin.UserId,
                CreatedDt = DateTime.Now,
            });

            return result;
        }

        public DownloadFileModel DownloadFile(DownloadFileRequest request) {
            var path = encryptionHelper.Decrypt(Base64UrlEncoder.Decode(request.Path));
            var token = encryptionHelper.Decrypt(Base64UrlEncoder.Decode(request.Token));
            var result = new DownloadFileModel();

            var currentLogin = userAccountHelper.ValidateToken(token);

            if (currentLogin != null) result.SanitizedPath = SanitizePath(path, true);
            else return null;

            var latestAuditLog = jsonHelper.GetLatestAuditLog();

            var auditLog = new AuditLog {  
                Api = MethodBase.GetCurrentMethod().Name,
                Request = $"Path: {path}",
                Response = null,
                Exception = null,
                ClientIp = userAccountHelper.GetClientIp(),
                CreatedBy = currentLogin != null ? currentLogin.UserId : 0,
                CreatedDt = DateTime.Now,
            };

            if (latestAuditLog.Api != auditLog.Api ||
                latestAuditLog.Request != auditLog.Request ||
                latestAuditLog.ClientIp != auditLog.ClientIp ||
                latestAuditLog.CreatedBy != auditLog.CreatedBy ||
                latestAuditLog.CreatedDt <= DateTime.Now.AddHours(-3)) {
                    jsonHelper.AddAuditList(auditLog);
                }

            return result;
        }
        
        public GenerateDownloadModel GenerateDownload(GenerateDownloadRequest request, SanitizePathModel sanitizedPath = null) {
            var sanitizedItem = sanitizedPath ?? SanitizePath(request.Path, false);
            var tokenLastHour = 4;
            var tokenLastMin = 0;
            var expiryDt = DateTime.Now.AddHours(tokenLastHour).AddMinutes(tokenLastMin);
            var result = new GenerateDownloadModel {
                Link = $"{rootUrl}api/File/DownloadFile/{Base64UrlEncoder.Encode(encryptionHelper.Encrypt(sanitizedItem.RelativePath))}/{Base64UrlEncoder.Encode(encryptionHelper.Encrypt(userAccountHelper.GenerateTempToken(tokenLastHour, tokenLastMin)))}/{sanitizedItem.Name.Replace("~", "_")}", // url cannot handle ~ correctly hence replace with _
                GeneratedDt = DateTime.Now,
                ExpiryDt = expiryDt,
            };

            // jsonHelper.AddAuditList(new AuditLog {  
            //     Api = MethodBase.GetCurrentMethod().Name,
            //     Request = jsonHelper.ToJson(request),
            //     Response = null,
            //     Exception = null,
            //     ClientIp = userAccountHelper.GetClientIp(),
            //     CreatedBy = userAccountHelper.CurrentLogin.UserId,
            //     CreatedDt = DateTime.Now,
            // });

            return result;
        }
        public SanitizePathModel SanitizePath(string path, bool isAbsolutePath) {
            if(isAbsolutePath) path = path.Replace(rootPath, "");

            path = path.Replace("\\", "/");
            var pathArray = path.Split('/');
            path = String.Join("/", pathArray.Where(w => !String.IsNullOrWhiteSpace(w)));

            var systemPath = Path.GetFullPath($"{rootPath}{path}");

            bool isDirectory = false;
            string extension = null;
            bool isPathChanged = false;

            while(true) {
                if (Directory.Exists(systemPath)) {
                    isDirectory = true;
                    break;
                }
                if (File.Exists(systemPath)) {
                    extension = Path.GetExtension(systemPath);
                    extension = String.IsNullOrWhiteSpace(extension) ? "" : extension.Substring(1);
                    extension = extension.ToLower();
                    break;
                }
                
                systemPath = Path.GetDirectoryName(systemPath);
                path = Path.GetDirectoryName(path);
                isPathChanged = true;
            }

            var fileName = Path.GetFileName(systemPath);

            var provider = new FileExtensionContentTypeProvider();
            string contentType = null;
            if (!isDirectory) {
                if(!provider.TryGetContentType(systemPath, out contentType))
                {
                    contentType = "application/octet-stream";
                }
            }
            var relativePath = path.Replace("\\", "/");

            var result = new SanitizePathModel {
                Path = systemPath,
                Name = fileName,
                RelativePath = relativePath,
                Type = contentType,
                Extension = extension,
                IsDirectory = isDirectory,
                IsPathChanged = isPathChanged,
                IsVideo = (extension == "mp4" || extension == "mkv"),
                IsInAnimeDirectory = relativePath.StartsWith("Anime/") || relativePath.StartsWith("AnimeEnd/"), //TODO: Move the array into config
                IsInAnimeEnd = relativePath == ("AnimeEnd"),
            };

            return result;
        }

        public SanitizedHiddenPathModel SanitizeHiddenPath (string path) {
            var result = new SanitizedHiddenPathModel() {
                Path = path,
            };
            var hiddenFileList = configHelper.HiddenFileList.Where(w => !w.WhitelistRole.Contains(userAccountHelper.CurrentLogin.RoleId));
            while (true) {
                if (hiddenFileList.Any(a => result.Path.StartsWith(a.Path))) {
                    result.Path = Path.GetDirectoryName(result.Path);
                    result.IsPathChanged = true;
                }
                else {
                    break;
                }
            }
            return result;
        }

        public ScanPathModel ScanPath(ScanPathRequest request) {
            // TODO: Enhance to return file size
            var sanitizedPath = SanitizePath(SanitizeHiddenPath(request.Path).Path, false);

            var result = SimpleScanPath(request, sanitizedPath);
            var immortalFileList = configHelper.ImmortalFileList;
            
            if (userAccountHelper.CurrentLogin.RoleId <= 1) { //Admin role
                result.IsFolderCreatable = result.IsDirectory;
                result.MovableFolderPath = result.IsDirectory ? GetMovableFolderPath(sanitizedPath) : new List<GetMovablePathModel>();
                result.isMovable = result.CurrentPath.Contains('/');
                result.IsDeletable = !immortalFileList.Any(a => a == result.CurrentPath);
                result.IsBookmarkable = !immortalFileList.Any(a => a == result.CurrentPath);
            } else {
                result.IsBookmarkable = result.IsDirectory && !immortalFileList.Any(a => a == result.CurrentPath);
                result.MovableFolderPath = new List<GetMovablePathModel>();
            }
            result.ShowFolderOption = result.IsDirectory && (result.IsFolderCreatable || result.isMovable || result.IsDeletable || result.IsBookmarkable);

            return result;
        }

        private ScanPathModel SimpleScanPath(ScanPathRequest request, SanitizePathModel sanitizedPath = null) {
            var result = new ScanPathModel();

            var folderList = new List<ContFolder>();
            var fileList = new List<ContFile>();

            sanitizedPath ??= SanitizePath(SanitizeHiddenPath(request.Path).Path, false);
            result.IsDirectory = sanitizedPath.IsDirectory;
            List<string> files = new();
            var watchedList = jsonHelper.WatchedList;
            var noteList = jsonHelper.NoteList;
            
            if (result.IsDirectory) {
                if (sanitizedPath.IsInAnimeEnd) {
                    DirectoryInfo info = new DirectoryInfo(sanitizedPath.Path);
                    var tempFiles = info.GetFiles().OrderByDescending(p => p.LastWriteTime).ToArray();
                    var tempDir = info.GetDirectories().OrderByDescending(p => p.LastWriteTime).ToArray();
                    files = tempDir.Select(s => s.FullName).Union(tempFiles.Select(s => s.FullName)).ToList();
                } else {
                    files = Directory.GetFileSystemEntries(sanitizedPath.Path).ToList();
                }
            } else {
                files.Add(sanitizedPath.Path);
            }

            Dictionary<string, string> animeNameList = null;
            Dictionary<string, string> subtitleList = null;
            
            foreach(var item in files) {
                var attr = File.GetAttributes(item);
                var lastAccessDate = File.GetLastWriteTime(item);
                var isNew = lastAccessDate >= DateTime.Now.AddDays(-3);
                var isDirectory = attr.HasFlag(FileAttributes.Directory);
                var sanitizedItem = SanitizePath(item, true);
                if (SanitizeHiddenPath(sanitizedItem.RelativePath).IsPathChanged) {
                    continue;
                }

                if (isDirectory) {
                    folderList.Add(new ContFolder{
                        Name = Path.GetFileName(item),
                        IsNew = isNew,
                    });
                } else {
                    var noteItem = noteList.Where(w => w.FileName == sanitizedItem.Name).FirstOrDefault();
                    var type = 0;

                    if (sanitizedItem.IsVideo) type = 1;
                    else if (sanitizedItem.Type == "application/json") type = 2;

                    string subtitleUrl = null, animeName = null;
                    if (type == 1) {
                        if (!result.IsDirectory) {
                            subtitleList ??= GetSubtitleList(sanitizedItem);
                            var getSubtitleResult = GetSubtitle(subtitleList, sanitizedItem, true);
                            if (getSubtitleResult.Result) subtitleUrl = getSubtitleResult.Url;
                        }
                        if (!sanitizedItem.IsInAnimeDirectory) {
                            animeNameList ??= GetAnimeNameList(files);
                            animeName = animeNameList.GetValueOrDefault(sanitizedItem.Name);
                        }
                    }

                    string fileLink = (result.IsDirectory ? null : GenerateDownload(new GenerateDownloadRequest(sanitizedPath.RelativePath), sanitizedItem).Link);

                    var contFile = new ContFile {
                        Name = sanitizedItem.Name,
                        Type = type,
                        IsNew = isNew,
                        IsWatched = watchedList.Any(a => a.FileName == sanitizedItem.Name && a.CreatedBy == userAccountHelper.CurrentLogin.UserId && a.IsWatched),
                        Note = noteItem != null ? noteItem.Note : "",
                        Link = fileLink,
                        SubtitleUrl = subtitleUrl,
                        AnimeName = animeName,
                    };

                    if (result.IsDirectory) {
                        subtitleList ??= GetSubtitleList(sanitizedItem);
                        contFile.fileListProp= new FileListPropModel {
                            ShowDownloadButton = sanitizedItem.IsVideo,
                            ShowWatchedButton = sanitizedItem.IsVideo,
                            ShowDownloadSubtitleButton = sanitizedItem.IsVideo && GetSubtitle(subtitleList, sanitizedItem, false).Result,
                        };
                    };

                    fileList.Add(contFile);
                }
            }
            
            result.WindowsPlayer = !String.IsNullOrWhiteSpace(configHelper.WindowsPlayer) ? configHelper.WindowsPlayer : "MPCBE";
            result.WindowsUrlScheme = !String.IsNullOrWhiteSpace(configHelper.WindowsUrlScheme) ? configHelper.WindowsUrlScheme : "mpcbe://";
            result.FolderList = folderList;
            result.FileList = fileList;
            result.CurrentPath = sanitizedPath.RelativePath;
            
            return result;
        }

        public GetSubtitleModel GetSubtitle(string path) {
            var sanitizedItem = SanitizePath(path, false);
            return GetSubtitle(GetSubtitleList(sanitizedItem), sanitizedItem, true);
        }

        private Dictionary<string, string> GetSubtitleList(SanitizePathModel model) {
            var result = new Dictionary<string, string>();
            if (configHelper.SubtitleExtension == null) return result;
            try {
                var subsExt = configHelper.SubtitleExtension.Select(s => $".{s}");
                var videoFile = Path.GetFileNameWithoutExtension(model.Name);
                var parent = Directory.GetParent(model.Path);
                
                string subsPath = Path.Combine(parent.FullName, "subs");
                bool isSubsPathExists = Directory.Exists(subsPath);

                var tempResult = new List<string>();
                foreach(var item in subsExt) {
                    tempResult.AddRange(Directory.GetFileSystemEntries(parent.FullName).Where(w => Path.GetExtension(w) == item));
                    if (isSubsPathExists)
                        tempResult.AddRange(Directory.GetFileSystemEntries(subsPath).Where(w => Path.GetExtension(w) == item));
                }

                foreach (var item in tempResult) {
                    var subNameWithoutExtension = Path.GetFileNameWithoutExtension(item);
                    if (!result.ContainsKey(subNameWithoutExtension)) {
                        result.Add(subNameWithoutExtension, item);
                    }
                }
            } catch (Exception) { }

            return result;
        }

        private GetSubtitleModel GetSubtitle(Dictionary<string, string> subtitleList, SanitizePathModel model, bool toGenerateUrl) {
            var result = new GetSubtitleModel();
            if (subtitleList.Count() == 0) return result;
            try {
                var videoFile = Path.GetFileNameWithoutExtension(model.Name);

                var langAffix = new List<string>() { "" };
                if (configHelper.SubtitleAffix != null) {
                    foreach(var item in configHelper.SubtitleAffix) {
                        langAffix.Add($".{item}");
                        langAffix.Add($".{item.ToUpper()}");
                    }
                }

                string correctSubsPath = null;
                foreach(var item in langAffix) {
                    correctSubsPath = subtitleList.GetValueOrDefault($"{videoFile}{item}");
                    if (!String.IsNullOrWhiteSpace(correctSubsPath)) break;
                }
                
                if (!String.IsNullOrWhiteSpace(correctSubsPath)) {
                    result.Result = true;

                    if (toGenerateUrl) {
                        var sanitizeSubsPath = SanitizePath(correctSubsPath, true);
                        result.Url = GenerateDownload(new GenerateDownloadRequest(sanitizeSubsPath.RelativePath), sanitizeSubsPath).Link;
                    }
                }
            } catch (Exception) { }
            
            return result;
        }
        
        public SubmitMoveFileModel SubmitMoveFile (SubmitMoveFileRequest request) {
            var result = new SubmitMoveFileModel();
            var movedStatus = false;
            var source = SanitizePath(request.SourceFile, false);
            var target = SanitizePath(request.TargetPath, false);

            dynamic exception = null;
            try {
                if (source.RelativePath != request.SourceFile) {
                    throw new FileNotFoundException($"Source File Not Found");
                }
                if (target.RelativePath != request.TargetPath) {
                    throw new DirectoryNotFoundException($"Target Path Not Found");
                }
                var targetFile = Path.Combine(target.Path, source.Name);
                if (source.IsDirectory) {
                    Directory.Move(source.Path, targetFile);
                } else {
                    File.Move(source.Path, targetFile);
                }
                movedStatus = true;

                if (source.IsVideo) {
                    var sanitizedTargetFile = SanitizePath(targetFile, true);
                    SimpleSubmitWatched(sanitizedTargetFile, true);
                }
            } catch (Exception ex) {
                exception = new {
                    Exception = ex.Message,
                    Request = request,
                };
            }

            var movedList = jsonHelper.MovedList;
            movedList.Add(new MovedFile{
                FileName = source.Name,
                SourcePath = Path.GetDirectoryName(source.Path),
                TargetPath = target.Path,
                Status = movedStatus,
                Exception = exception,
                CreatedBy = userAccountHelper.CurrentLogin.UserId,
                CreatedDt = DateTime.Now,
            });
            jsonHelper.SaveMovedList(movedList);

            result.Result = movedStatus;

            jsonHelper.AddAuditList(new AuditLog {  
                Api = MethodBase.GetCurrentMethod().Name,
                Request = jsonHelper.ToJson(request),
                Response = jsonHelper.ToJson(result),
                Exception = jsonHelper.ToJson(exception),
                ClientIp = userAccountHelper.GetClientIp(),
                CreatedBy = userAccountHelper.CurrentLogin.UserId,
                CreatedDt = DateTime.Now,
            });

            return result;
        }

        public SubmitCreateMoveFileModel SubmitCreateMoveFile (SubmitCreateMoveFileRequest request) {
            var result = new SubmitCreateMoveFileModel();

            if (CreateNewFolder(new CreateNewFolderRequest { Path = "Anime", Name = request.TargetPath }).Result) {
                var tempResult = SubmitMoveFile(new SubmitMoveFileRequest { SourceFile = request.SourceFile, TargetPath = "Anime/" + request.TargetPath });
                result.Result = tempResult.Result;
            }

            return result;
        }
    
        public GetJustMovedModel GetJustMoved () {
            var result = new GetJustMovedModel();
            var justMovedList = jsonHelper.MovedList
                                .Where(w => w.CreatedDt >= DateTime.Now.AddDays(-3) && w.Status)
                                .Select(s => 
                                    new GetJustMovedModel.File{
                                    Name = s.FileName,
                                    Path = SanitizePath(Path.Combine(s.TargetPath, s.FileName), true).RelativePath,
                                }).ToList();
            result.FileList = justMovedList;

            return result;
        }
    
        public CreateNewFolderModel CreateNewFolder (CreateNewFolderRequest request) {
            var result = new CreateNewFolderModel();
            
            var createStatus = false;
            var target = SanitizePath(request.Path, false);
            try {
                request.Path = request.Path.Trim();
                if (target.RelativePath != request.Path) {
                    throw new DirectoryNotFoundException($"Target Path Not Found");
                }
                if (!Directory.Exists(Path.Combine(target.Path, request.Name.Trim()))) {
                    Directory.CreateDirectory(Path.Combine(target.Path, request.Name.Trim()));
                }
                createStatus = true;
            } catch (Exception) {
            }
            result.Result = createStatus;
            return result;
        }

        public DeleteFileModel DeleteFile (DeleteFileRequest request) {
            var result = new DeleteFileModel();
            var immortalPathList = configHelper.ImmortalFileList;
            
            var deleteStatus = false;
            var target = SanitizePath(request.Path, false);

            dynamic exception = null;
            try {
                if (userAccountHelper.CurrentLogin.RoleId > 1 ) {
                    throw new SystemException($"Unauthorized Deletion");
                }
                if (target.RelativePath != request.Path) {
                    throw new FileNotFoundException($"Path Not Found");
                }
                if (immortalPathList.Contains(target.RelativePath)) {
                    throw new SystemException($"Immortal Path Detected");
                }

                if (target.IsDirectory) {
                    Directory.Delete(target.Path, true);
                } else {
                    File.Delete(target.Path);
                }
                deleteStatus = true;
            } catch (Exception ex) {
                exception = new {
                    Exception = ex.Message,
                    Request = request,
                };
            }

            result.Result = deleteStatus;

            jsonHelper.AddAuditList(new AuditLog {  
                Api = MethodBase.GetCurrentMethod().Name,
                Request = jsonHelper.ToJson(request),
                Response = jsonHelper.ToJson(result),
                Exception = jsonHelper.ToJson(exception),
                ClientIp = userAccountHelper.GetClientIp(),
                CreatedBy = userAccountHelper.CurrentLogin.UserId,
                CreatedDt = DateTime.Now,
            });
            
            return result;
        }

        public List<GetMovablePathModel> GetMovableFolderPath (SanitizePathModel model) {
            var result = new List<GetMovablePathModel>();
            var splitPath = model.RelativePath.Split('/');
            if (splitPath.Length == 0) return result;

            var firstDir = splitPath[0];
            if (firstDir == "AnimeDL") {
                result.Add(new GetMovablePathModel("Anime", "Anime"));
                result.Add(new GetMovablePathModel("AnimeEnd", "AnimeEnd"));
            } else if (firstDir == "Anime") {
                result.Add(new GetMovablePathModel("AnimeEnd", "AnimeEnd"));
            } else if (firstDir == "AnimeEnd") {
                result.Add(new GetMovablePathModel("Anime", "Anime"));
            }
            return result;
        }
    
        public List<GetMovablePathModel> GetMovableAnimePath () {
            var scanPathModel = SimpleScanPath(new ScanPathRequest { Path = "Anime"});
            var result = scanPathModel.FolderList.Select(s => {
              var newPath = Path.Combine(scanPathModel.CurrentPath, s.Name);
              var sanitizedItem = SanitizePath(newPath, false);
              return new GetMovablePathModel(sanitizedItem.Name, sanitizedItem.RelativePath);
            }).ToList();

            return result;
        }

        private Dictionary<string,string> GetAnimeNameList (List<string> files) {
            var tempResult = new ConcurrentDictionary<string, string>();
            try {
                var movedList = jsonHelper.MovedList.Where(w => w.CreatedDt >= DateTime.Today.AddMonths(-2) && w.Status).OrderByDescending(o => o.CreatedDt);
                
                Parallel.ForEach(files, item => {
                    var attr = File.GetAttributes(item);
                    if (attr.HasFlag(FileAttributes.Directory)) return;
                    var lowestScore = Int32.MaxValue;
                    var fileName = Path.GetFileName(item);
                    dynamic movedFile = null;
                    Parallel.ForEach(movedList, movedItem => {
                        var score = StringHelper.CheckSimilarity(fileName, movedItem.FileName);
                        if (score <= 3 && score < lowestScore) {
                            movedFile = movedItem;
                            lowestScore = score;
                            if (score == 1) return;
                        }
                    });
                    if (lowestScore < Int32.MaxValue) {
                        var sanitizedTargetPath = SanitizePath(movedFile.TargetPath, true);
                        if (!sanitizedTargetPath.IsPathChanged) tempResult.TryAdd(fileName, sanitizedTargetPath.Name);
                    }
                });
            } catch (Exception ex) {
                jsonHelper.AddAuditList(new AuditLog {  
                    Api = MethodBase.GetCurrentMethod().Name,
                    Request = jsonHelper.ToJson(new { files }),
                    Response = null,
                    Exception = ex.Message,
                    ClientIp = userAccountHelper.GetClientIp(),
                    CreatedBy = userAccountHelper.CurrentLogin.UserId,
                    CreatedDt = DateTime.Now,
                });
            }
            return new Dictionary<string, string>(tempResult, tempResult.Comparer);
        }

        public UploadTextFileModel UploadTextFile(UploadTextFileRequest request) {
            var result = new UploadTextFileModel();
            dynamic exception = null;
            try {
                var sanitizedItem = SanitizePath(request.Path, false);
                if (sanitizedItem.IsPathChanged) throw new Exception("File not existed");
                if (!jsonHelper.IsValidJsonString(request.Content)) throw new Exception("Content is not a valid json");
                File.Copy(sanitizedItem.Path, $"{sanitizedItem.Path}.bak", true);
                File.WriteAllText(sanitizedItem.Path, request.Content);
                result.Result = true;
            } catch (Exception ex) {
                exception = new {
                    Exception = ex.Message,
                    Request = request,
                };
            }
            jsonHelper.AddAuditList(new AuditLog {  
                Api = MethodBase.GetCurrentMethod().Name,
                Request = jsonHelper.ToJson(new { request.Path, Content = "" }),
                Response = jsonHelper.ToJson(result),
                Exception = jsonHelper.ToJson(exception),
                ClientIp = userAccountHelper.GetClientIp(),
                CreatedBy = userAccountHelper.CurrentLogin.UserId,
                CreatedDt = DateTime.Now,
            });

            return result;
        }
    }
}