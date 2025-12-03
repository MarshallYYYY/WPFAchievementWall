using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using Server.Data;
using System.Diagnostics;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Users?userName=xxx&password=xxx
        [HttpGet]
        public async Task<ActionResult<User>> GetUserForLogin(string userName, string password)
        {
            // 在本次查询中，让 UserName 区分大小写
            User? user = await _context.Users.FirstOrDefaultAsync(
                u => EF.Functions.Collate(u.UserName, "Chinese_PRC_CS_AS") == userName);

            // 其实最好不要区分用户不存在和密码错误，统一返回401即可。
            if (user is null)
            {
                // 404
                return NotFound("用户名不存在！");
            }
            // 这种方式就直接可以区分大小写了，因为是比较的字符串
            if (user.Password != password)
            {
                // 400
                //return BadRequest("密码输入错误！");
                // 或：401
                return Unauthorized("密码输入错误！");
            }

            // 200
            return Ok(user);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                //return CreatedAtAction("GetUser", new { id = user.Id }, user);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException)
                {
                    //2627 — Violation of PRIMARY KEY or UNIQUE KEY constraint（违反主键或唯一约束）
                    //2601 — Cannot insert duplicate key row in object with unique index（违反唯一索引）
                    // 经过测试，实际获取的是 2601
                    if (sqlException.Number == 2627 || sqlException.Number == 2601)
                    {
                        // Conflict() = 409 状态码，表示资源冲突
                        //return Conflict(new { message = "用户名已存在！" });
                        return Conflict("用户名已存在！");
                    }
                }

                // 未知异常，返回 500 或 BadRequest（视需求）
                // 不要把内部异常原文返回给客户端用于安全考虑，可记录日志
                // Log the exception (not shown here)
                //return BadRequest(new { message = ex.InnerException?.Message });
                return BadRequest(ex.InnerException?.Message);
                // 使用 Problem() 返回标准的 RFC 7807 错误格式
                //return Problem(
                //    statusCode: StatusCodes.Status500InternalServerError,
                //    title: "服务器错误"
                //);
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}