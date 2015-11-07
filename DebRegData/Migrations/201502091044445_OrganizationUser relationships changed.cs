namespace DebRegData.Migrations {
    using System.Data.Entity.Migrations;

    public partial class OrganizationUserrelationshipschanged : DbMigration {
        public override void Up() {
            DropForeignKey("dbo.OrganizationUsers", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.OrganizationUsers", new[] { "User_Id" });
            DropColumn("dbo.OrganizationUsers", "User_Id");
            //AddForeignKey("dbo.OrganizationUsers", "UserId", "dbo.AspNetUsers", "Id");
        }

        public override void Down() {
            //DropForeignKey("dbo.OrganizationUsers", "UserId", "dbo.AspNetUsers", "Id");
            AddColumn("dbo.OrganizationUsers", "User_Id", c => c.String(nullable: true, maxLength: 128));
            CreateIndex("dbo.OrganizationUsers", "User_Id");
            AddForeignKey("dbo.OrganizationUsers", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
