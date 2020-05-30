namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewDb : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropForeignKey("dbo.ObservableUsers", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropIndex("dbo.ObservableUsers", new[] { "ObservableUser_Id" });
            DropIndex("dbo.ObservableUsers", new[] { "ApplicationUser_Id" });
            CreateTable(
                "dbo.ObservableUserObservableUsers",
                c => new
                    {
                        ObservableUser_Id = c.Int(nullable: false),
                        ObservableUser_Id1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ObservableUser_Id, t.ObservableUser_Id1 })
                .ForeignKey("dbo.ObservableUsers", t => t.ObservableUser_Id)
                .ForeignKey("dbo.ObservableUsers", t => t.ObservableUser_Id1)
                .Index(t => t.ObservableUser_Id)
                .Index(t => t.ObservableUser_Id1);
            
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
            
            DropColumn("dbo.ObservableUsers", "ObservableUser_Id");
            DropColumn("dbo.ObservableUsers", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ObservableUsers", "ApplicationUser_Id", c => c.Int());
            AddColumn("dbo.ObservableUsers", "ObservableUser_Id", c => c.Int());
            DropForeignKey("dbo.ObservableUserApplicationUsers", "ApplicationUser_Id", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ObservableUserApplicationUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropForeignKey("dbo.ObservableUserObservableUsers", "ObservableUser_Id1", "dbo.ObservableUsers");
            DropForeignKey("dbo.ObservableUserObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropIndex("dbo.ObservableUserApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ObservableUserApplicationUsers", new[] { "ObservableUser_Id" });
            DropIndex("dbo.ObservableUserObservableUsers", new[] { "ObservableUser_Id1" });
            DropIndex("dbo.ObservableUserObservableUsers", new[] { "ObservableUser_Id" });
            DropTable("dbo.ObservableUserApplicationUsers");
            DropTable("dbo.ObservableUserObservableUsers");
            CreateIndex("dbo.ObservableUsers", "ApplicationUser_Id");
            CreateIndex("dbo.ObservableUsers", "ObservableUser_Id");
            AddForeignKey("dbo.ObservableUsers", "ApplicationUser_Id", "dbo.ApplicationUsers", "Id");
            AddForeignKey("dbo.ObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers", "Id");
        }
    }
}
