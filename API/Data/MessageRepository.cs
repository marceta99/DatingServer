using API.DTOS;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);  
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message); 
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages
                .Include(u =>u.Sender)
                .Include(u =>u.Recipient)
                .SingleOrDefaultAsync(x => x.Id == id); 
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages.           //vracacemo prvo najnovije poruke
                OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            switch (messageParams.Container)
            {
                case "Inbox": //slucaj kada smo mi ulogovani user i trazimo poruke koje su poslate nama
                    query = query.Where(u => u.Recipient.UserName == messageParams.Username && u.RecipientDeleted == false);
                    break;
                case "Outbox": //slucaj kada smo mi ulogovani user i trazimo poruke koje smo mi poslali
                    query = query.Where(u => u.Sender.UserName == messageParams.Username && u.SenderDeleted == false); 
                    break;
                default:
                    query = query.Where(u => u.Recipient.UserName == messageParams.Username && 
                            u.DateRead == null && u.RecipientDeleted == false); //null znaci da jos uvek nije procitao ovu poruku
                    break; 
            }

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize); 



        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername,string recipientUsername)
        {
           //hocemo da prikazemo sve poruke koje je curentUser slao recipientu , i sve poruke koje je recipient slao currentUseru
            var messages = await _context.Messages
                .Include(u => u.Recipient).ThenInclude(p=>p.Photos)//hocemo u svakoj razmeni poruka da imamo pristup slikama usera 
                .Include(u => u.Sender).ThenInclude(p=>p.Photos)
                .Where(m => m.Recipient.UserName == currentUsername && m.Sender.UserName == recipientUsername && m.RecipientDeleted == false
                      || m.Recipient.UserName == recipientUsername && m.Sender.UserName == currentUsername && m.SenderDeleted == false)
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            //sve poruke koje trenutni korisnk nije procitao u razgovoru sa recipientom
            var undreadMessages = messages.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername);

            //hocemo sad posto vracamo sve poruke izmedju njih, hocemo da oznacimo da je sada sve poruke currentUser procitao
            if (undreadMessages.Any())
            {
                foreach(var message in undreadMessages)
                {
                    message.DateRead = DateTime.Now; 
                }
                await _context.SaveChangesAsync(); //ovde cuvamo te promene u bazi , tj sve te poruke koje su sad procitane
            }

            return _mapper.Map<IEnumerable<MessageDTO>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;  
        }
    }
}
