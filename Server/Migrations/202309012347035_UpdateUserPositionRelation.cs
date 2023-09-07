namespace Server.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UpdateUserPositionRelation : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.UserEntity", new[] { "LasPosition_Id" });
            RenameColumn(table: "dbo.UserEntity", name: "LasPosition_Id", newName: "LasPositionId");
            AlterColumn("dbo.UserEntity", "LasPositionId", c => c.Guid(nullable: false));
            CreateIndex("dbo.UserEntity", "LasPositionId");
        }

        public override void Down()
        {
            DropIndex("dbo.UserEntity", new[] { "LasPositionId" });
            AlterColumn("dbo.UserEntity", "LasPositionId", c => c.Guid());
            RenameColumn(table: "dbo.UserEntity", name: "LasPositionId", newName: "LasPosition_Id");
            CreateIndex("dbo.UserEntity", "LasPosition_Id");
        }
    }
}
