using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Cont5.Models.Config;

namespace Cont5.Helpers
{
    public interface IConfigHelper
    {
        string RootUrl { get; }
        string JwtSecret { get; }
        string RsaPublicKey { get; }
        string RsaPrivateKey { get; }
        string RootPath { get; }
        string AriaNgUrl { get; }
        string UserJsonPath { get; }
        string WatchedJsonPath { get; }
        string NoteJsonPath { get; }
        string MovedJsonPath { get; }
        string AuditJsonPath { get; }
        string BookmarkJsonPath { get; }
        string ImmortalFileJsonPath { get; }
        List<string> ImmortalFileList { get; }
        string HiddenFileJsonPath { get; }
        string FailedDownloadFileJsonPath { get; }
        List<HiddenFile> HiddenFileList { get; }
        List<string> SubtitleAffix { get; }
        List<string> SubtitleExtension { get; }
        int BlockIpLoginAttempts { get; }
        string WindowsPlayer { get; }
        string WindowsUrlScheme { get; }
    }
    public class ConfigHelper : IConfigHelper
    {
        private readonly IConfiguration config;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ConfigHelper(IConfiguration config, IHttpContextAccessor httpContextAccessor) {
            this.config = config;
            this.httpContextAccessor = httpContextAccessor;
        }
        public string RootUrl {
            get => $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/";
        }
        public string JwtSecret {
            get => config.GetValue<string>("JwtSecret");
        }
        public string RsaPublicKey {
            get => config.GetValue<string>("RsaPublicKey");
        }
        public string RsaPrivateKey {
            get => config.GetValue<string>("RsaPrivateKey");
        }

        public string RootPath {
            get => config.GetValue<string>("RootPath");
        }

        public string AriaNgUrl {
            get => config.GetValue<string>("AriaNgUrl");
        }

        public string FailedDownloadFileJsonPath {
            get => Path.Combine(RootPath, config.GetValue<string>("FailedDownloadFileJsonPath"));
        }
        
        public string UserJsonPath {
            get => config.GetValue<string>("UserJsonPath");
        }
        public string WatchedJsonPath {
            get => config.GetValue<string>("WatchedJsonPath");
        }
        public string NoteJsonPath {
            get => config.GetValue<string>("NoteJsonPath");
        }
        public string MovedJsonPath {
            get => config.GetValue<string>("MovedJsonPath");
        }
        public string AuditJsonPath {
            get => config.GetValue<string>("AuditJsonPath");
        }

        public string BookmarkJsonPath {
            get => config.GetValue<string>("BookmarkJsonPath");
        }

        public string ImmortalFileJsonPath {
            get => config.GetValue<string>("ImmortalFileJsonPath");
        }
        public List<string> ImmortalFileList {
            get {
                var immortalFilePath = this.ImmortalFileJsonPath;
                var json = File.ReadAllText(immortalFilePath);

                var result = JsonConvert.DeserializeObject<List<string>>(json);
                if (result == null)  return new List<string>();

                return result;
            }
        }

        public string HiddenFileJsonPath {
            get => config.GetValue<string>("HiddenFileJsonPath");
        }
        public List<HiddenFile> HiddenFileList {
            get {
                var hiddenFilePath = this.HiddenFileJsonPath;
                var json = File.ReadAllText(hiddenFilePath);

                var result = JsonConvert.DeserializeObject<List<HiddenFile>>(json);
                if (result == null)  return new List<HiddenFile>();

                return result;
            }
        }
        public List<string> SubtitleAffix {
            get => config.GetSection("SubtitleAffix").Get<List<string>>();
        }
        public List<string> SubtitleExtension {
            get => config.GetSection("SubtitleExtension").Get<List<string>>();
        }
        public int BlockIpLoginAttempts {
            get => config.GetValue<int?>("BlockIpLoginAttempts") ?? 3;
        }
        public string WindowsPlayer {
            get => config.GetValue<string>("WindowsPlayer");
        }
        public string WindowsUrlScheme {
            get => config.GetValue<string>("WindowsUrlScheme");
        }
    }
}