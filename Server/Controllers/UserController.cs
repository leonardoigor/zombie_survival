using Core.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Server.Entities;
using Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {


        private readonly ILogger<UserController> _logger;
        private readonly UserRepository userRepository;

        public UserController(ILogger<UserController> logger, UserRepository userRepository)
        {
            _logger = logger;
            this.userRepository = userRepository;
        }

        [HttpGet]
        public IEnumerable<UserEntity> Get()
        {
            return userRepository.List().ToList();
        }
        [HttpPost]
        public async Task<bool> Create(CreateUserDto user)
        {
            try
            {
                _logger.LogInformation($"{nameof(Create)} Started");
                userRepository.Add(user.toDto());
                var result = await userRepository.SaveChanges();
                var res = result != 0 ? "Success" : "Error";
                _logger.LogInformation($"{nameof(Create)} {res}");
                return result != 0;
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(Create)} Error: {e.Message}");

                return false;
            }

        }
    }
}
