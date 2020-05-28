namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewFields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropIndex("dbo.ObservableUsers", new[] { "ObservableUser_Id" });
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
            
            DropColumn("dbo.ObservableUsers", "ObservableUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ObservableUsers", "ObservableUser_Id", c => c.Int());
            DropForeignKey("dbo.ObservableUserObservableUsers", "ObservableUser_Id1", "dbo.ObservableUsers");
            DropForeignKey("dbo.ObservableUserObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropIndex("dbo.ObservableUserObservableUsers", new[] { "ObservableUser_Id1" });
            DropIndex("dbo.ObservableUserObservableUsers", new[] { "ObservableUser_Id" });
            DropTable("dbo.ObservableUserObservableUsers");
            CreateIndex("dbo.ObservableUsers", "ObservableUser_Id");
            AddForeignKey("dbo.ObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers", "Id");
        }
    }
}
