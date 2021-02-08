namespace College_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subject : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Subject", "TeacherId", "dbo.Teacher");
            DropIndex("dbo.Subject", new[] { "TeacherId" });
            AlterColumn("dbo.Subject", "TeacherId", c => c.Int());
            CreateIndex("dbo.Subject", "TeacherId");
            AddForeignKey("dbo.Subject", "TeacherId", "dbo.Teacher", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Subject", "TeacherId", "dbo.Teacher");
            DropIndex("dbo.Subject", new[] { "TeacherId" });
            AlterColumn("dbo.Subject", "TeacherId", c => c.Int(nullable: false));
            CreateIndex("dbo.Subject", "TeacherId");
            AddForeignKey("dbo.Subject", "TeacherId", "dbo.Teacher", "Id", cascadeDelete: true);
        }
    }
}
