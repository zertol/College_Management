namespace College_Management.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Course",
                c => new
                    {
                        CourseId = c.Int(nullable: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.CourseId);
            
            CreateTable(
                "dbo.Subject",
                c => new
                    {
                        SubjectId = c.Int(nullable: false),
                        CourseId = c.Int(nullable: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.SubjectId)
                .ForeignKey("dbo.Course", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.Enrollment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SubjectId = c.Int(nullable: false),
                        StudentId = c.Int(nullable: false),
                        Grade = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Subject", t => t.SubjectId, cascadeDelete: true)
                .ForeignKey("dbo.Student", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.SubjectId)
                .Index(t => t.StudentId);
            
            CreateTable(
                "dbo.Teacher",
                c => new
                    {
                        SubjectId = c.Int(nullable: false),
                        Name = c.String(),
                        Birthday = c.DateTime(nullable: false),
                        Salary = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SubjectId)
                .ForeignKey("dbo.Subject", t => t.SubjectId)
                .Index(t => t.SubjectId);
            
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Birthday = c.DateTime(nullable: false),
                        RegNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enrollment", "StudentId", "dbo.Student");
            DropForeignKey("dbo.Subject", "CourseId", "dbo.Course");
            DropForeignKey("dbo.Teacher", "SubjectId", "dbo.Subject");
            DropForeignKey("dbo.Enrollment", "SubjectId", "dbo.Subject");
            DropIndex("dbo.Teacher", new[] { "SubjectId" });
            DropIndex("dbo.Enrollment", new[] { "StudentId" });
            DropIndex("dbo.Enrollment", new[] { "SubjectId" });
            DropIndex("dbo.Subject", new[] { "CourseId" });
            DropTable("dbo.Student");
            DropTable("dbo.Teacher");
            DropTable("dbo.Enrollment");
            DropTable("dbo.Subject");
            DropTable("dbo.Course");
        }
    }
}
