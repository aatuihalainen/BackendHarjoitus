using BackendHarjoitus.Models;
using BackendHarjoitus.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BackendHarjoitus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService userService)
        {
            _service = userService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            return Ok(await _service.GetUsersAsync());
        }

        // GET: api/Users/5
        [HttpGet("{username}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetUser(string username)
        {
            var user = await _service.GetUserAsync(username);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{username}")]
        [Authorize]
        public async Task<IActionResult> PutUser(string username, User user)
        {
            string loggedInUserName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (loggedInUserName != username)
            {
                return Unauthorized();
            }

            if (username != user.Username)
            {
                return BadRequest("Username does not match");
            }

            if (await _service.UpdateUserAsync(user))
            {
                return NoContent();
            }

            return NotFound();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(User user)
        {
            User? newUser = await _service.CreateUserAsync(user);
            if (newUser == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetUser), new {username = newUser.Username}, newUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{username}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string username)
        {
            string loggedInUserName = this.User.FindFirst(ClaimTypes.Name).Value;
            if (loggedInUserName != username)
            {
                return Unauthorized();
            }

            if (await _service.DeleteUserAsync(username))
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
