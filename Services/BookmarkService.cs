using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cont5.Models.Bookmark;
using Cont5.Helpers;
using Cont5.Models.Json;
using System.Reflection;

namespace Cont5.Services {
    public interface IBookmarkService {
        AddBookmarkModel AddBookmark(AddBookmarkRequest request);
        DeleteBookmarkModel DeleteBookmark(DeleteBookmarkRequest request);
        List<GetBookmarkModel> GetBookmark();
    }

    public class BookmarkService : IBookmarkService {
        private readonly IJsonHelper jsonHelper;
        private readonly IUserAccountHelper userAccountHelper;
        private readonly IFileService fileService;

        public BookmarkService(IJsonHelper jsonHelper, IUserAccountHelper userAccountHelper, IFileService fileService) {
            this.jsonHelper = jsonHelper;
            this.userAccountHelper = userAccountHelper;
            this.fileService = fileService;
        }

        public AddBookmarkModel AddBookmark(AddBookmarkRequest request) {
            var result = new AddBookmarkModel();
            dynamic exception = null;
            try {
                var sanitizedPath = fileService.SanitizePath(request.Path, false);
                if (sanitizedPath.IsPathChanged) throw new Exception("Path not found");
                if (fileService.SanitizeHiddenPath(sanitizedPath.RelativePath).IsPathChanged) throw new Exception("Path is not visible by this user");
                if (!sanitizedPath.IsDirectory) throw new Exception("Path is not a directory");

                var bookmarkList = jsonHelper.Bookmark;
                var toSaveBookmark = true;

                var existingBookmark = bookmarkList.FirstOrDefault(f => f.Path == sanitizedPath.RelativePath && f.CreatedBy == userAccountHelper.CurrentLogin.UserId);
                if (existingBookmark != null) {
                    if (existingBookmark.IsDeleted) {
                        existingBookmark.IsDeleted = false;
                        existingBookmark.UpdatedDt = DateTime.Now;
                    } else {
                        toSaveBookmark = false;
                        result.Result = true;
                    }
                } else {
                    bookmarkList.Add(new Bookmark{
                        Path = sanitizedPath.RelativePath,
                        CreatedBy = userAccountHelper.CurrentLogin.UserId,
                        CreatedDt = DateTime.Now,
                    });
                }
                if (toSaveBookmark) result.Result = jsonHelper.SaveBookmark(bookmarkList);
            } catch (Exception ex) {
                exception = new {
                    Exception = ex.Message,
                    Request = request,
                };
            }
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

        public DeleteBookmarkModel DeleteBookmark(DeleteBookmarkRequest request) {
            var result = new DeleteBookmarkModel();
            dynamic exception = null;
            try {
                var sanitizedPath = fileService.SanitizePath(request.Path, false);
                if (sanitizedPath.IsPathChanged) throw new Exception("Path not found");
                if (fileService.SanitizeHiddenPath(sanitizedPath.RelativePath).IsPathChanged) throw new Exception("Path is not visible by this user");
                if (!sanitizedPath.IsDirectory) throw new Exception("Path is not a directory");

                var bookmarkList = jsonHelper.Bookmark;
                var toSaveBookmark = true;

                var existingBookmark = bookmarkList.FirstOrDefault(f => f.Path == sanitizedPath.RelativePath && f.CreatedBy == userAccountHelper.CurrentLogin.UserId);
                if (existingBookmark != null) {
                    if (!existingBookmark.IsDeleted) {
                        existingBookmark.IsDeleted = true;
                        existingBookmark.UpdatedDt = DateTime.Now;
                    } else {
                        toSaveBookmark = false;
                    }
                } else throw new Exception($"Bookmark Not Found");

                if (toSaveBookmark) result.Result = jsonHelper.SaveBookmark(bookmarkList);
            } catch (Exception ex) {
                exception = new {
                    Exception = ex.Message,
                    Request = request,
                };
            }
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

        public List<GetBookmarkModel> GetBookmark() {
            var result = new List<GetBookmarkModel>();
            var bookmarkList = jsonHelper.Bookmark.Where(w => w.CreatedBy == userAccountHelper.CurrentLogin.UserId && !w.IsDeleted).ToList();
            var toSaveBookmark = false;
            foreach(var item in bookmarkList) {
                if (fileService.SanitizeHiddenPath(item.Path).IsPathChanged) continue; // skip hidden path

                var sanitizedPath = fileService.SanitizePath(item.Path, false);
                if (sanitizedPath.IsPathChanged) {
                    item.IsDeleted = true;
                    item.UpdatedDt = DateTime.Now;
                    toSaveBookmark = true;
                } else {
                    if (!sanitizedPath.IsDirectory) continue; // skip if not a directory
                    result.Add(new GetBookmarkModel {
                        Name = sanitizedPath.Name,
                        Path = sanitizedPath.RelativePath,
                    });
                }
            }
            if (toSaveBookmark) {
                jsonHelper.SaveBookmark(bookmarkList);
            }
            return result.OrderBy(o => o.Name).ToList();
        }
    }
}