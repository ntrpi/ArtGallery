namespace ArtGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArtGallery_202103032100 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Images", "imageOldName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Images", "imageOldName");
        }
    }
}
