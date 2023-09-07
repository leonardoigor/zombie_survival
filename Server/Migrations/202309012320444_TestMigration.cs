namespace Server.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class TestMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserEntity",
                c => new
                {
                    Id = c.Guid(nullable: false),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.UserEntity");
        }
    }
}
