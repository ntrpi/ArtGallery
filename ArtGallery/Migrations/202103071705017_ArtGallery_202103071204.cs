namespace ArtGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArtGallery_202103071204 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Forms", "formDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Forms", "formDescription");
        }
    }
}
