namespace Server.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddUserLastIp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserEntity", "LastIP", c => c.String(maxLength: 100, unicode: false));
        }

        public override void Down()
        {
            DropColumn("dbo.UserEntity", "LastIP");
        }
    }
}
