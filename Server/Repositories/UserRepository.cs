using Server.Entities;
using Server.Infra;
using Server.Repositories.Base;
using System;

namespace Server.Repositories
{
    public class UserRepository : RepositoryBase<UserEntity, Guid>
    {
        public UserRepository(SurvivalContext db) : base(db) { }


    }
}
