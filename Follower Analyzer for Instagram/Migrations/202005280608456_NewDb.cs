namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewDb : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ObservableUserApplicationUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropForeignKey("dbo.ObservableUserApplicationUsers", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ObservableUserApplicationUsers", new[] { "ObservableUser_Id" });
            DropIndex("dbo.ObservableUserApplicationUsers", new[] { "ApplicationUser_Id" });
            AddColumn("dbo.ObservableUsers", "ApplicationUser_Id", c => c.Int());
            AddColumn("dbo.UserActivities", "ObserverPrimaryKey", c => c.String());
            AddColumn("dbo.UserActivities", "TargetUserPrimaryKey", c => c.String());
            CreateIndex("dbo.ObservableUsers", "ApplicationUser_Id");
            AddForeignKey("dbo.ObservableUsers", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
            DropColumn("dbo.UserActivities", "TargetUserName");
            DropTable("dbo.ObservableUserApplicationUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ObservableUserApplicationUsers",
                c => new
                    {
                        ObservableUser_Id = c.Int(nullable: false),
                        ApplicationUser_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ObservableUser_Id, t.ApplicationUser_Id });
            
            AddColumn("dbo.UserActivities", "TargetUserName", c => c.String());
            DropForeignKey("dbo.ObservableUsers", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ObservableUsers", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.UserActivities", "TargetUserPrimaryKey");
            DropColumn("dbo.UserActivities", "ObserverPrimaryKey");
            DropColumn("dbo.ObservableUsers", "ApplicationUser_Id");
            CreateIndex("dbo.ObservableUserApplicationUsers", "ApplicationUser_Id");
            CreateIndex("dbo.ObservableUserApplicationUsers", "ObservableUser_Id");
            AddForeignKey("dbo.ObservableUserApplicationUsers", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ObservableUserApplicationUsers", "ObservableUser_Id", "dbo.ObservableUsers", "Id", cascadeDelete: true);
        }
    }
}
