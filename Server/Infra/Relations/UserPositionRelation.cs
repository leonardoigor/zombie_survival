using Server.Entities;
using System.Data.Entity;

namespace Server.Infra.Relations
{
    public static class UserPositionRelation
    {
        public static DbModelBuilder AddUserPositionRelation(this DbModelBuilder m)
        {

            m.Entity<UserEntity>()
                .HasRequired(e => e.LasPosition)
                .WithMany()
                .HasForeignKey(e => e.LasPositionId);
            return m;
        }
    }
}
