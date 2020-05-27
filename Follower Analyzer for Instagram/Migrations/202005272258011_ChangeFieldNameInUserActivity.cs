namespace Follower_Analyzer_for_Instagram.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeFieldNameInUserActivity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserActivities", "TargetUserName", c => c.String());
            DropColumn("dbo.UserActivities", "TargetUserPrimaryKey");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserActivities", "TargetUserPrimaryKey", c => c.String());
            DropColumn("dbo.UserActivities", "TargetUserName");
        }
    }
}
