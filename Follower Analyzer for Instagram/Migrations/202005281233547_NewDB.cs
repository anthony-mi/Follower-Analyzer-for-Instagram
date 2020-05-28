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
                        ObserverPrimaryKey = c.String(),
                        InitiatorPrimaryKey = c.String(),
                        TargetUserPrimaryKey = c.String(),
                        EventDate = c.DateTime(nullable: false),
                        LinkToMedia = c.String(),
                        ActivityType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ObservableUserApplicationUsers",
                c => new
                    {
                        ObservableUser_Id = c.Int(nullable: false),
                        ApplicationUser_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ObservableUser_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.ObservableUsers", t => t.ObservableUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.ApplicationUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.ObservableUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUsers", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ObservableUserApplicationUsers", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ObservableUserApplicationUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropForeignKey("dbo.ObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropIndex("dbo.ObservableUserApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ObservableUserApplicationUsers", new[] { "ObservableUser_Id" });
            DropIndex("dbo.ObservableUsers", new[] { "ObservableUser_Id" });
            DropIndex("dbo.ApplicationUsers", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ObservableUserApplicationUsers");
            DropTable("dbo.UserActivities");
            DropTable("dbo.ObservableUsers");
            DropTable("dbo.ApplicationUsers");
        }
    }
}
