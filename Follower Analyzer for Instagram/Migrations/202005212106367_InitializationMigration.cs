namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitializationMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        InstagramPK = c.String(nullable: false, maxLength: 128),
                        StateData = c.Binary(),
                        FollowersCount = c.Int(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        User_InstagramPK = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.InstagramPK)
                .ForeignKey("dbo.Users", t => t.User_InstagramPK)
                .Index(t => t.User_InstagramPK);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "User_InstagramPK", "dbo.Users");
            DropIndex("dbo.Users", new[] { "User_InstagramPK" });
            DropTable("dbo.Users");
        }
    }
}
