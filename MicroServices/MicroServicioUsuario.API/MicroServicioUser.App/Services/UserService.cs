using MicroServicioUser.Dom.Entities;
using MicroServicioUser.Dom.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroServicioUser.App.Services
{
    public class UserService
    {
        private readonly IRepository userRepository;
        public UserService(IRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<int> Insert(User t)
        {
            var id = (int)(await userRepository.Insert(t));
            return id;
        }

        public async Task<List<User>> Select()
        {
            return await userRepository.Select();
        }
    }
}
