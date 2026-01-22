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

        // Only customers can close chats.
        [Authorize(Roles = AppRoles.Customer)] 
        [HttpPatch("{id}/Close")]
        public async Task<IActionResult>CloseChat(int id)
        {
            var result = await chatService.CloseChat(id);
            if (result) return Ok(ApiResponse.SuccessResponse("Chat closed successfully", null));
            return BadRequest(ApiResponse.ErrorResponse("Failed to close the chat.", null));
        }

        // Both customers and sellers can send messages in an open chat.
        // We will only validate that the requestor is either a customer or a seller in the chat resource.
        [Authorize(Roles = $"{AppRoles.Customer},{AppRoles.Seller}")]
        [HttpPost("{id}/Message")]
        public async Task <IActionResult> SendMessage(int id, SendMessageDto sendMessageDto)
        {
            var result = await chatService.SendMessage(id, sendMessageDto);
            if (result) return Ok(ApiResponse.SuccessResponse("Message sent successfully", null));
            return BadRequest(ApiResponse.ErrorResponse("Failed to send a message.", null));
        }
        [Authorize(Roles = $"{AppRoles.Customer},{AppRoles.Seller}")]
        [HttpGet("{id}")]
        public async Task<IActionResult> FetchChat(int id)
        {
            var result = await chatService.GetChatAlongWithMessages(id);
            if (result != null) return Ok(ApiResponse.SuccessResponse("Chat fetched successfully", result));
            return NotFound(ApiResponse.ErrorResponse("Chat not found.", null));
        }
    }
}
