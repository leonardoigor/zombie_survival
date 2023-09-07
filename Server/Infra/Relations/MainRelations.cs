using System.Data.Entity;

namespace Server.Infra.Relations
{
    public static class MainRelations
    {
        public static DbModelBuilder AddRelations(this DbModelBuilder m)
        {
            m.AddUserPositionRelation();
            return m;
        }
    }
}

