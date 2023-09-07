using Microsoft.Extensions.Configuration;
using Server.Entities;
using Server.Infra.Relations;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Server.Infra
{
    public class SurvivalContext : DbContext
    {
        public SurvivalContext()
        {
        }

        public SurvivalContext(IConfiguration conf) : base(conf.GetValue<string>("ConnectionString"))
        {
        }

        #region dbModels
        public DbSet<UserEntity> User { get; set; }
        public DbSet<PositionEntity> Position { get; set; }

        #endregion
        #region config
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.AddRelations();
            //Remove the pluralization of table names
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Remove cascading delete
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            //Set to use varchar or instead of nvarchar
            modelBuilder.Properties<string>().Configure(p => p.HasColumnType("varchar"));
            modelBuilder.Properties<double>().Configure(p => p.HasColumnType("float"));


            //If I forget to inform the field size, it will put a varchar of 100
            modelBuilder.Properties<string>().Configure(p => p.HasMaxLength(100));


            #region Add Entity auto-map by assembly
            modelBuilder.Configurations.AddFromAssembly(typeof(SurvivalContext).Assembly);
            #endregion

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}

