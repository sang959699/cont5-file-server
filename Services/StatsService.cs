using Cont5.Models.File;
using Cont5.Helpers;
using System;
using System.Linq;
using Cont5.Models.Json;
using System.Reflection;
using Cont5.Models.Stats;

namespace Cont5.Services {
    public interface IStatsService {
        GetStatsModel GetWatchedStats();
        GetStatsModel GetPendingStats();
    }

    public class StatsService : IStatsService {
        private readonly IUserAccountHelper userAccountHelper;
        private readonly IJsonHelper jsonHelper;
        private readonly IFileService fileService;

        public StatsService(IUserAccountHelper userAccountHelper, IJsonHelper jsonHelper, IFileService fileService) {
            this.userAccountHelper = userAccountHelper;
            this.jsonHelper = jsonHelper;
            this.fileService = fileService;
        }

        private static string GetFormattedTime(long ms) {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);
            return string.Format(t.Days > 0 ? "{3:D2}:{0:D2}:{1:D2}:{2:D2}" : "{0:D2}:{1:D2}:{2:D2}", 
                    t.Hours,
                    t.Minutes,
                    t.Seconds,
                    t.Days);
        }

        public GetStatsModel GetWatchedStats() {
            var result = new GetStatsModel();
            dynamic exception = null;
            try {
                var watchedList = jsonHelper.WatchedList;
                var ytd = watchedList.Where(w => w.IsWatched && w.CreatedBy == userAccountHelper.CurrentLogin.UserId && w.CreatedDt.Year == DateTime.Today.Year).Select(s => new { s.CreatedDt, s.Duration });
                var threeMonth = watchedList.Where(w => w.IsWatched && w.CreatedBy == userAccountHelper.CurrentLogin.UserId && DateTime.Today.AddMonths(-3) <= w.CreatedDt.Date).OrderByDescending(o => o.CreatedDt).Select(s => new { s.CreatedDt, s.Duration });
                var oneMonth = threeMonth.Where(w => DateTime.Today.AddMonths(-1) <= w.CreatedDt.Date);
                var oneWeek = oneMonth.Where(w => DateTime.Today.AddDays(-7) <= w.CreatedDt.Date);
                var yesterday = oneWeek.Where(w => DateTime.Today.AddDays(-1) == w.CreatedDt.Date);
                var today = oneWeek.Where(w => w.CreatedDt.Date == DateTime.Today);

                result.Data.Add(new string[] { "Today", GetFormattedTime(today.Sum(s => s.Duration)), today.Count().ToString() });
                result.Data.Add(new string[] { "Yesterday", GetFormattedTime(yesterday.Sum(s => s.Duration)), yesterday.Count().ToString() });
                result.Data.Add(new string[] { "1 Week", GetFormattedTime(oneWeek.Sum(s => s.Duration)), oneWeek.Count().ToString() });
                result.Data.Add(new string[] { "1 Month", GetFormattedTime(oneMonth.Sum(s => s.Duration)), oneMonth.Count().ToString() });
                result.Data.Add(new string[] { "3 Months", GetFormattedTime(threeMonth.Sum(s => s.Duration)), threeMonth.Count().ToString() });
                result.Data.Add(new string[] { "YTD", GetFormattedTime(ytd.Sum(s => s.Duration)), ytd.Count().ToString() });
            } catch (Exception ex) {
                exception = new {
                    Exception = ex.Message,
                };

                jsonHelper.AddAuditList(new AuditLog {  
                    Api = MethodBase.GetCurrentMethod().Name,
                    Response = jsonHelper.ToJson(result),
                    Exception = jsonHelper.ToJson(exception),
                    ClientIp = userAccountHelper.GetClientIp(),
                    CreatedBy = userAccountHelper.CurrentLogin.UserId,
                    CreatedDt = DateTime.Now,
                });
            }

            return result;
        }

        public GetStatsModel GetPendingStats() {
            var result = new GetStatsModel();
            try {
                var animeDLList = fileService.ScanPath(new ScanPathRequest { Path = "AnimeDL"});
                double duration = 0;
                int pendingWatchEpCount = 0;
                foreach(var item in animeDLList.FileList) {
                    if (item.Type == 1) {
                        using (var mediaInfo = new MediaInfo.MediaInfo()) {
                            // TODO: Enhance to handle exception in loop
                            var sanitizedItem = fileService.SanitizePath($"{animeDLList.CurrentPath}/{item.Name}", false);
                            mediaInfo.Open(sanitizedItem.Path);
                            duration += Convert.ToDouble(mediaInfo.Get(MediaInfo.StreamKind.Video, 0, "Duration"));
                        }
                        pendingWatchEpCount++;
                    }
                }
                result.Data.Add(new string[] { "Pending", GetFormattedTime(((long)duration)), pendingWatchEpCount.ToString() });
            } catch (Exception ex) {
                dynamic exception = new {
                    Exception = ex.Message,
                };

                jsonHelper.AddAuditList(new AuditLog {  
                    Api = MethodBase.GetCurrentMethod().Name,
                    Response = jsonHelper.ToJson(result),
                    Exception = jsonHelper.ToJson(exception),
                    ClientIp = userAccountHelper.GetClientIp(),
                    CreatedBy = userAccountHelper.CurrentLogin.UserId,
                    CreatedDt = DateTime.Now,
                });
            }

            return result;
        }
    }
}