namespace ArtGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArtGallery_202102251639 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Forms",
                c => new
                    {
                        formId = c.Int(nullable: false, identity: true),
                        formName = c.String(),
                    })
                .PrimaryKey(t => t.formId);
            
            AddColumn("dbo.Pieces", "formId", c => c.Int(nullable: false));
            CreateIndex("dbo.Pieces", "formId");
            AddForeignKey("dbo.Pieces", "formId", "dbo.Forms", "formId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pieces", "formId", "dbo.Forms");
            DropIndex("dbo.Pieces", new[] { "formId" });
            DropColumn("dbo.Pieces", "formId");
            DropTable("dbo.Forms");
        }
    }
}
