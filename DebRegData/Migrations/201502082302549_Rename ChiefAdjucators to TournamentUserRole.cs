namespace DebRegData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameChiefAdjucatorstoTournamentUserRole : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ChiefAdjudicators", newName: "TournamentUserRoles");
            AddColumn("dbo.TournamentUserRoles", "Role", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TournamentUserRoles", "Role");
            RenameTable(name: "dbo.TournamentUserRoles", newName: "ChiefAdjudicators");
        }
    }
}
