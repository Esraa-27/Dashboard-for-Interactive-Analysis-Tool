using Market_Repositry.Data;
using MarketCore.Entities;
using MarketCore.Repositries;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketRepositry
{
    public class UserRepo : IUserRepo
    {
        private readonly MarketContext context;

        public UserRepo(MarketContext _context)
        {
             context = _context;
        }

        public AppUser GetUserByToken(string token)
            => context.Users.Where(user => user.Token == token).FirstOrDefault();

    }
}
