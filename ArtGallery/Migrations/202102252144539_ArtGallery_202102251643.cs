namespace ArtGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ArtGallery_202102251643 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Pieces", "formId", "dbo.Forms");
            DropIndex("dbo.Pieces", new[] { "formId" });
            AddColumn("dbo.Pieces", "Form_formId", c => c.Int());
            AlterColumn("dbo.Pieces", "formId", c => c.Int());
            CreateIndex("dbo.Pieces", "formId");
            CreateIndex("dbo.Pieces", "Form_formId");
            AddForeignKey("dbo.Pieces", "Form_formId", "dbo.Forms", "formId");
            AddForeignKey("dbo.Pieces", "formId", "dbo.Forms", "formId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pieces", "formId", "dbo.Forms");
            DropForeignKey("dbo.Pieces", "Form_formId", "dbo.Forms");
            DropIndex("dbo.Pieces", new[] { "Form_formId" });
            DropIndex("dbo.Pieces", new[] { "formId" });
            AlterColumn("dbo.Pieces", "formId", c => c.Int(nullable: false));
            DropColumn("dbo.Pieces", "Form_formId");
            CreateIndex("dbo.Pieces", "formId");
            AddForeignKey("dbo.Pieces", "formId", "dbo.Forms", "formId", cascadeDelete: true);
        }
    }
}
