using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cont5.Models.File;
using Microsoft.AspNetCore.Authorization;
using Cont5.Services;
using Cont5.Helpers;
using System;

namespace Cont5.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService fileService;
        private readonly IUserAccountHelper userAccountHelper;

        public FileController(IFileService fileService, IUserAccountHelper userAccountHelper)
        {
            this.fileService = fileService;
            this.userAccountHelper = userAccountHelper;
        }

        [AllowAnonymous]
        [HttpGet("DownloadFile/{path}/{token}/{fileName?}")]
        public async Task<IActionResult> DownloadFile(string path, string token)
        {
            var downloadFileModel = await Task.Run(() => fileService.DownloadFile(new DownloadFileRequest{ Path = path, Token = token }));
            if (downloadFileModel.SanitizedPath != null) {
                var result = File(System.IO.File.OpenRead(downloadFileModel.SanitizedPath.Path), downloadFileModel.SanitizedPath.Type);
                result.EnableRangeProcessing = true;
                result.FileDownloadName = downloadFileModel.SanitizedPath.Name;
                return result;
            }
            return BadRequest();
        }

        [HttpPost("GenerateDownload")]
        public async Task<IActionResult> GenerateDownload(GenerateDownloadRequest request)
        {
            try {
                var result = await Task.Run(() => fileService.GenerateDownload(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("ScanPath")]
        public async Task<IActionResult> ScanPath(ScanPathRequest request)
        {
            try {
                var result = await Task.Run(() => fileService.ScanPath(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("GetMovableAnimePath")]
        public async Task<IActionResult> GetMovableAnimePath()
        {
            try {
                var result = await Task.Run(() => fileService.GetMovableAnimePath());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("SubmitWatched")]
        public async Task<IActionResult> SubmitWatched(SubmitWatchedRequest request)
        {
            try {
                var result = await Task.Run(() => fileService.SubmitWatched(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("SubmitNote")]
        public async Task<IActionResult> SubmitNote(SubmitNoteRequest request)
        {
            try {
                var result = await Task.Run(() => fileService.SubmitNote(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("SubmitMoveFile")]
        public async Task<IActionResult> SubmitMoveFile(SubmitMoveFileRequest request)
        {
            try {
                if (userAccountHelper.CurrentLogin.RoleId > 1) throw new Exception("User Not Authorized");
                
                var result = await Task.Run(() => fileService.SubmitMoveFile(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("SubmitCreateMoveFile")]
        public async Task<IActionResult> SubmitCreateMoveFile(SubmitCreateMoveFileRequest request)
        {
            try {
                if (userAccountHelper.CurrentLogin.RoleId > 1) throw new Exception("User Not Authorized");
                
                var result = await Task.Run(() => fileService.SubmitCreateMoveFile(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("GetJustMoved")]
        public async Task<IActionResult> GetJustMoved()
        {
            try {
                var result = await Task.Run(() => fileService.GetJustMoved());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("CreateNewFolder")]
        public async Task<IActionResult> CreateNewFolder(CreateNewFolderRequest request)
        {
            try {
                if (userAccountHelper.CurrentLogin.RoleId > 1) throw new Exception("User Not Authorized");

                var result = await Task.Run(() => fileService.CreateNewFolder(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("DeleteFile")]
        public async Task<IActionResult> DeleteFile(DeleteFileRequest request)
        {
            try {
                if (userAccountHelper.CurrentLogin.RoleId > 1) throw new Exception("User Not Authorized");

                var result = await Task.Run(() => fileService.DeleteFile(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("GetSubtitle")]
        public async Task<IActionResult> GetSubtitle(GetSubtitleRequest request)
        {
            try {
                var result = await Task.Run(() => fileService.GetSubtitle(request.Path));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("UploadTextFile")]
        public async Task<IActionResult> UploadTextFile(UploadTextFileRequest request)
        {
            try {
                if (userAccountHelper.CurrentLogin.RoleId > 1) throw new Exception("User Not Authorized");

                var result = await Task.Run(() => fileService.UploadTextFile(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }
    }
}
