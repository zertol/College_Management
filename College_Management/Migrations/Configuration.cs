namespace College_Management.Migrations
{
    using College_Management.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<College_Management.DAL.CollegeContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(College_Management.DAL.CollegeContext context)
        {
            
        }
    }
}
