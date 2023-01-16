using Microsoft.AspNetCore.Mvc;
using CommonLib.Models;
using UserServiceLib;
using Microsoft.EntityFrameworkCore;
using BookService.Models.DTO;
using BookService.Repositories;

namespace BookService.Controllers
{
    [Route("api/v1/digitalbooks/[controller]")]
    [ApiController]
    public class PublisherController : Controller
    {
        private readonly IPublisherRepository _publisherRepository;
        public PublisherController(IPublisherRepository publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllPublisher()
        {
            var publishers = await _publisherRepository.GetAllPublisherAsync(); 

            return Ok(publishers);
        }

        [HttpPost]
        public async Task<IActionResult> AddPublishers([FromBody] Publisher publisher)
        {
            var publisherRes = new Publisher()
            {
                PublisherName = publisher.PublisherName                
            };
            var publisherResult = await _publisherRepository.AddPublisherAsync(publisher);

            return Ok(publisherResult);
        }  
    }
}
