using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendHarjoitus.Models;
using BackendHarjoitus.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BackendHarjoitus.Middleware;

namespace BackendHarjoitus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _service;
        private readonly IUserAuthenticationService _userAuthenticationService;

        public MessagesController(IMessageService messageService, IUserAuthenticationService userAuthenticationService)
        {
            _service = messageService;
            _userAuthenticationService = userAuthenticationService;
        }

        // GET: api/Messages
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages()
        {
            return Ok(await _service.GetMessagesAsync()); 
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<MessageDTO>> GetMessage(long id)
        {

            string username = this.User.FindFirst(ClaimTypes.Name).Value;

            if (!await _userAuthenticationService.CanUserViewMessage(username, id))
            {
                return Unauthorized();
            }

            var message = await _service.GetMessageAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutMessage(long id, MessageDTO message)
        {
            string username = this.User.FindFirst(ClaimTypes.Name).Value;
            message.Id = id;
            if (!await _userAuthenticationService.IsMyMessage(username, id)){
                return Unauthorized();
            }

            if (await _service.UpdateMessageAsync(message))
            {
                return NoContent();
            }
            
            return BadRequest();
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<MessageDTO>> PostMessage(MessageDTO message)
        {
            message.SenderId = this.User.FindFirst(ClaimTypes.Name).Value;
            MessageDTO ? newMessage = await _service.CreateMessageAsync(message);

            if (newMessage == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(GetMessage), new { id = newMessage.Id }, newMessage);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(long id)
        {
            string username = this.User.FindFirst(ClaimTypes.Name).Value;
            if (!await _userAuthenticationService.IsMyMessage(username, id))
            {
                return Unauthorized();
            }

            var message = await _service.GetMessageAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            await _service.DeleteMessageAsync(id);
            return NoContent();
        }
    }
}
