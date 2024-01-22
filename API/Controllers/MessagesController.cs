using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace API.Controllers
{
    public class MessagesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public MessagesController(IMapper mapper, IUnitOfWork uow)
        {
            _mapper = mapper;
            _uow = uow;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto) {
            var username = User.GetUsername();

            if (username == createMessageDto.ReceipentUsername.ToLower()) {
                return BadRequest("You cannot send messages to yourself!");
            }

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var receipent = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.ReceipentUsername);

            if (receipent == null) return NotFound();

            var message = new Message {
                // EF knows that sender has sender.id since it is AppUser object and it automatically set it but only for the Id
                Sender = sender,
                Receipent = receipent,
                SenderUsername = sender.UserName,
                ReceipentUsername = receipent.UserName,
                Content = createMessageDto.Content
            };

            _uow.MessageRepository.AddMessage(message);

            if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send a message!");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams) {
            messageParams.Username = User.GetUsername();

            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        // [HttpGet("thread/{username}")]
        // public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username) {
        //     var currentUserName = User.GetUsername();

        //     return Ok(await _uow.MessageRepository.GetMessageThread(currentUserName, username));
        // }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id) {
            var username = User.GetUsername();

            var message = await _uow.MessageRepository.GetMessage(id);

            // ensure this current user that wants to delete message is either sender or receipent of the message
            if (message.SenderUsername != username && message.ReceipentUsername != username) return Unauthorized();

            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.ReceipentUsername == username) message.ReceipentDeleted = true;

            // check if both sender and receipent delete their message, only then we delete message from db
            if (message.SenderDeleted && message.ReceipentDeleted) {
                _uow.MessageRepository.DeleteMessage(message);
            }

            if (await _uow.Complete()) return Ok();

            return BadRequest("Problem deleting the message!");
        }
    }
}