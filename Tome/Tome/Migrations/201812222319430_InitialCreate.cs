namespace Tome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tomes", "Name", c => c.String(nullable: false));
            DropColumn("dbo.TomeHistories", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TomeHistories", "Name", c => c.String());
            AlterColumn("dbo.Tomes", "Name", c => c.String());
        }
    }
}
