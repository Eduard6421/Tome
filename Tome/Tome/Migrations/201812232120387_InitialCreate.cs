namespace Tome.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CurrentVersions",
                c => new
                    {
                        CurrentVersionId = c.Int(nullable: false, identity: true),
                        TomeId = c.Int(nullable: false),
                        TomeHistoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CurrentVersionId)
                .ForeignKey("dbo.Tomes", t => t.TomeId, cascadeDelete: true)
                .ForeignKey("dbo.TomeHistories", t => t.TomeHistoryId, cascadeDelete: true)
                .Index(t => t.TomeId)
                .Index(t => t.TomeHistoryId);
            
            CreateTable(
                "dbo.Tomes",
                c => new
                    {
                        TomeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 30),
                        CreationDate = c.DateTime(nullable: false),
                        IsPrivate = c.Boolean(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.TomeId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.TomeHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
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
                        TagTitle = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagReferences", "TomeId", "dbo.Tomes");
            DropForeignKey("dbo.TagReferences", "TagId", "dbo.Tags");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.CurrentVersions", "TomeHistoryId", "dbo.TomeHistories");
            DropForeignKey("dbo.TomeHistories", "TomeId", "dbo.Tomes");
            DropForeignKey("dbo.TomeHistories", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CurrentVersions", "TomeId", "dbo.Tomes");
            DropForeignKey("dbo.Tomes", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.TagReferences", new[] { "TagId" });
            DropIndex("dbo.TagReferences", new[] { "TomeId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TomeHistories", new[] { "UserId" });
            DropIndex("dbo.TomeHistories", new[] { "TomeId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Tomes", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.CurrentVersions", new[] { "TomeHistoryId" });
            DropIndex("dbo.CurrentVersions", new[] { "TomeId" });
            DropTable("dbo.Tags");
            DropTable("dbo.TagReferences");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.TomeHistories");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Tomes");
            DropTable("dbo.CurrentVersions");
        }
    }
}
