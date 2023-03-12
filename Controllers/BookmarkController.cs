using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Cont5.Services;
using Cont5.Models.Bookmark;
using System;

namespace Cont5.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookmarkController : ControllerBase
    {
        private readonly IBookmarkService bookmarkService;

        public BookmarkController(IBookmarkService bookmarkService)
        {
            this.bookmarkService = bookmarkService;
        }

        [HttpPost("AddBookmark")]
        public async Task<IActionResult> AddBookmark(AddBookmarkRequest request)
        {
            try {
                var result = await Task.Run(() => bookmarkService.AddBookmark(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("DeleteBookmark")]
        public async Task<IActionResult> DeleteBookmark(DeleteBookmarkRequest request)
        {
            try {
                var result = await Task.Run(() => bookmarkService.DeleteBookmark(request));
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }

        [HttpPost("GetBookmark")]
        public async Task<IActionResult> GetBookmark()
        {
            try {
                var result = await Task.Run(() => bookmarkService.GetBookmark());
                return Ok(result);
            } catch (Exception) {
                return BadRequest();
            }
        }
    }
}
