using API.DTOS;
using API.Entities;
using API.Extentions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    //[Authorize]
    public class MessagesController  : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository , IMessageRepository messageRepository, IMapper mapper)
        {
            this._userRepository = userRepository;
            this._messageRepository = messageRepository;
            this._mapper = mapper;
        }

        [HttpPost("{senderUsername}")]
        public async Task<ActionResult<MessageDTO>> CreateMessage(string senderUsername ,CreateMessageDTO createMessageDTO)
        {
            if(senderUsername == createMessageDTO.RecipientUsername.ToLower())
            {
                return BadRequest("You cannot send messages to yourself"); 
            }

            var sender = await _userRepository.GetUserByUserNameAsync(senderUsername); 
            var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDTO.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient ,
                SenderUserName = sender.UserName , 
                RecipientUserName = recipient.UserName, 
                Content = createMessageDTO.Content
            };
            _messageRepository.AddMessage(message); //dodajemo novu poruku u bazu 

            //vracamo messageDTO kao response klijentu :  
            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to send message"); 

        }

        [HttpGet("{currentUserName}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser(string currentUserName
            ,[FromQuery] MessageParams messageParams)
        {
            messageParams.Username = currentUserName;

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages ;



        }
    
    
        [HttpGet("thread/{currentUsername}/{recipientUsername}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            return Ok(await _messageRepository.GetMessageThread(currentUserName, recipientUserName)); 
        }
    
        [HttpDelete("{currentUserName}/{messageId}")]
        public async Task<ActionResult> DeleteMessage(string currentUSerName , int messageId)
        {
            var message =await _messageRepository.GetMessage(messageId); 

            if(message.Sender.UserName != currentUSerName && message.Recipient.UserName != currentUSerName)
            {
                return Unauthorized(); //ako trenutni korisnik nije primalac , a nije ni posiljalac te poruke, onda ne moze da je
                                       //obrise
            }
            //ako npr samo onaj koji je poslao poruku hoce da je obrise onda cemo da stavimo prop senderDeleted = false
            //i zbog nje vise nece ta poruka da se prikazuje na ekranu sendera
            if (message.Sender.UserName == currentUSerName) message.SenderDeleted = true;

            if (message.Recipient.UserName == currentUSerName) message.RecipientDeleted = true;

            //poruku brisemo iz baze AKO I SAMO AKO i sender i recipient hoce da je obrisu. Ako samo jedan od njih hoce da je
            //obrise onda mu samo ne saljemo vise tu poruku ali ona ce i dalje biti u bazi sve dok i drugi ne odluci da je obrise
            if (message.SenderDeleted && message.RecipientDeleted) _messageRepository.DeleteMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem with deleting message");
        }
    
    }
}
