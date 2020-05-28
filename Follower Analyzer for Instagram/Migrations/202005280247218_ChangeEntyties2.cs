namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeEntyties2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ObservableUsers", "ObservableUser_Id", c => c.Int());
            CreateIndex("dbo.ObservableUsers", "ObservableUser_Id");
            AddForeignKey("dbo.ObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ObservableUsers", "ObservableUser_Id", "dbo.ObservableUsers");
            DropIndex("dbo.ObservableUsers", new[] { "ObservableUser_Id" });
            DropColumn("dbo.ObservableUsers", "ObservableUser_Id");
        }
    }
}
