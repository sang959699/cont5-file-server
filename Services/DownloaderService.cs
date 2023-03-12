using Cont5.Models.Downloader;
using Cont5.Helpers;

namespace Cont5.Services {
    public interface IDownloaderService {
        GetAriaNgUrlModel GetAriaNgUrl();
        GetFailedDownloadCountModel GetFailedDownloadCount();
    }

    public class DownloaderService : IDownloaderService {
        private readonly IConfigHelper configHelper;
        private readonly IJsonHelper jsonHelper;

        public DownloaderService(IConfigHelper configHelper, IJsonHelper jsonHelper) {
            this.configHelper = configHelper;
            this.jsonHelper = jsonHelper;
        }

        public GetAriaNgUrlModel GetAriaNgUrl() {
            var result = new GetAriaNgUrlModel{
                Url = configHelper.AriaNgUrl,
            };
            return result;
        }
        public GetFailedDownloadCountModel GetFailedDownloadCount() {
            var result = new GetFailedDownloadCountModel();
            result.Result = jsonHelper.FailedDownloadFileList.Count;
            return result;
        }
    }
}