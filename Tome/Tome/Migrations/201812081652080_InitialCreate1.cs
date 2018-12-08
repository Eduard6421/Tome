namespace Tome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagReferences",
                c => new
                    {
                        TagReferenceId = c.Int(nullable: false, identity: true),
                        TomeId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TagReferenceId)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .ForeignKey("dbo.Tomes", t => t.TomeId, cascadeDelete: true)
                .Index(t => t.TomeId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        TagTitle = c.String(),
                    })
                .PrimaryKey(t => t.TagId);
            
            CreateTable(
                "dbo.Tomes",
                c => new
                    {
                        TomeId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        IsPrivate = c.Boolean(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.TomeId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TomeHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ModificationDate = c.DateTime(nullable: false),
                        TomeId = c.Int(nullable: false),
                        FilePath = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Tomes", t => t.TomeId, cascadeDelete: true)
                .Index(t => t.TomeId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TomeHistories", "TomeId", "dbo.Tomes");
            DropForeignKey("dbo.TomeHistories", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TagReferences", "TomeId", "dbo.Tomes");
            DropForeignKey("dbo.Tomes", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TagReferences", "TagId", "dbo.Tags");
            DropIndex("dbo.TomeHistories", new[] { "UserId" });
            DropIndex("dbo.TomeHistories", new[] { "TomeId" });
            DropIndex("dbo.Tomes", new[] { "UserId" });
            DropIndex("dbo.TagReferences", new[] { "TagId" });
            DropIndex("dbo.TagReferences", new[] { "TomeId" });
            DropTable("dbo.TomeHistories");
            DropTable("dbo.Tomes");
            DropTable("dbo.Tags");
            DropTable("dbo.TagReferences");
        }
    }
}
