using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {

        private readonly AppDbContext _context;

        public TestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("database")]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var userCount = await _context.Users.CountAsync();
                var achievementCount = await _context.Achievements.CountAsync();

                return Ok(new
                {
                    Message = "数据库连接成功",
                    UserCount = userCount,
                    AchievementCount = achievementCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "数据库连接失败", Error = ex.Message });
            }
        }
    }
}
