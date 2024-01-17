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
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto) {
            var username = User.GetUsername();

            if (username == createMessageDto.ReceipentUsername.ToLower()) {
                return BadRequest("You cannot send messages to yourself!");
            }

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var receipent = await _userRepository.GetUserByUsernameAsync(createMessageDto.ReceipentUsername);

            if (receipent == null) return NotFound();

            var message = new Message {
                // EF knows that sender has sender.id since it is AppUser object and it automatically set it but only for the Id
                Sender = sender,
                Receipent = receipent,
                SenderUsername = sender.UserName,
                ReceipentUsername = receipent.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send a message!");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams) {
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username) {
            var currentUserName = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUserName, username));
        }
    }
}