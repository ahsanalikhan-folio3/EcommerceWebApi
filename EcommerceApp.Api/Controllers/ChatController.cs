using EcommerceApp.Application.Common;
using EcommerceApp.Application.Dtos;
using EcommerceApp.Application.Interfaces.Chats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;
        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        // Only customers can create chats.
        // Only customers can message first.
        [Authorize(Roles = AppRoles.Customer)] 
        [HttpPost]
        public async Task<IActionResult> CreateChat(CreateChatDto createChatDto)
        {
            var result = await chatService.CreateChat(createChatDto);
            if (result) return Ok(ApiResponse.SuccessResponse("Chat created successfully", null));
            return BadRequest(ApiResponse.ErrorResponse("Failed to create a chat.", null));
        }
    }
}
