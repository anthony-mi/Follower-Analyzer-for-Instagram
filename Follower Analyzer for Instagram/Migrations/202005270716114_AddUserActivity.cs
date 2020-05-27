namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserActivity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TargetUserPrimaryKey = c.String(),
                        InitiatorPrimaryKey = c.String(),
                        EventDate = c.DateTime(nullable: false),
                        LinkToMedia = c.String(),
                        ActivityType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserActivities");
        }
    }
}
