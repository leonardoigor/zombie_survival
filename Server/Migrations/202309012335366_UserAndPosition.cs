namespace Server.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UserAndPosition : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PositionEntity",
                c => new
                {
                    Id = c.Guid(nullable: false),
                    PX = c.Double(nullable: false),
                    PY = c.Double(nullable: false),
                    PZ = c.Double(nullable: false),
                    RX = c.Double(nullable: false),
                    RY = c.Double(nullable: false),
                    RZ = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.UserEntity", "SocketId", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.UserEntity", "UserName", c => c.String(maxLength: 100, unicode: false));
            AddColumn("dbo.UserEntity", "LasPosition_Id", c => c.Guid());
            CreateIndex("dbo.UserEntity", "LasPosition_Id");
            AddForeignKey("dbo.UserEntity", "LasPosition_Id", "dbo.PositionEntity", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.UserEntity", "LasPosition_Id", "dbo.PositionEntity");
            DropIndex("dbo.UserEntity", new[] { "LasPosition_Id" });
            DropColumn("dbo.UserEntity", "LasPosition_Id");
            DropColumn("dbo.UserEntity", "UserName");
            DropColumn("dbo.UserEntity", "SocketId");
            DropTable("dbo.PositionEntity");
        }
    }
}
