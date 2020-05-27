namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StateData = c.Binary(),
                        LastUpdateDate = c.DateTime(nullable: false),
                        InstagramPK = c.String(),
                        Username = c.String(),
                        ApplicationUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.ObservableUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InstagramPK = c.String(),
                        Username = c.String(),
                        ObservableUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ObservableUsers", t => t.ObservableUser_Id)
                .Index(t => t.ObservableUser_Id);
            
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
            DropForeignKey("dbo.ObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropForeignKey("dbo.ApplicationUsers", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ObservableUsers", new[] { "ObservableUser_Id" });
            DropIndex("dbo.ApplicationUsers", new[] { "ApplicationUser_Id" });
            DropTable("dbo.UserActivities");
            DropTable("dbo.ObservableUsers");
            DropTable("dbo.ApplicationUsers");
        }
    }
}
