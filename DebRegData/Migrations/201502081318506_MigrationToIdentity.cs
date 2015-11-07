namespace DebRegData.Migrations {
    using System.Data.Entity.Migrations;

    public partial class MigrationToIdentity : DbMigration {
        public override void Up() {
            DropForeignKey("dbo.Users", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Users", "ModifiedById", "dbo.Users");
            DropForeignKey("dbo.Tournaments", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Organizations", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Organizations", "ModifiedById", "dbo.Users");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "ModifiedById", "dbo.Users");
            DropForeignKey("dbo.OrganizationUsers", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.OrganizationUsers", "ModifiedById", "dbo.Users");
            DropForeignKey("dbo.OrganizationUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Tournaments", "ModifiedById", "dbo.Users");
            DropForeignKey("dbo.ChiefAdjudicators", "UserId", "dbo.Users");
            DropForeignKey("dbo.OrganizationUsers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Addresses", "CreatedById", "dbo.Users");
            DropForeignKey("dbo.Addresses", "ModifiedById", "dbo.Users");
            DropIndex("dbo.Addresses", new[] { "CreatedById" });
            DropIndex("dbo.Addresses", new[] { "ModifiedById" });

            RenameTable(name: "dbo.Users", newName: "AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "EMail" });
            DropIndex("dbo.AspNetUsers", new[] { "CreatedById" });
            DropIndex("dbo.AspNetUsers", new[] { "ModifiedById" });
            DropIndex("dbo.ChiefAdjudicators", new[] { "UserId" });
            DropIndex("dbo.Tournaments", new[] { "CreatedById" });
            DropIndex("dbo.Tournaments", new[] { "ModifiedById" });
            DropIndex("dbo.Organizations", new[] { "CreatedById" });
            DropIndex("dbo.Organizations", new[] { "ModifiedById" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "CreatedById" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "ModifiedById" });
            DropIndex("dbo.OrganizationUsers", new[] { "UserId" });
            DropIndex("dbo.OrganizationUsers", new[] { "CreatedById" });
            DropIndex("dbo.OrganizationUsers", new[] { "ModifiedById" });
            DropIndex("dbo.OrganizationUsers", new[] { "User_Id" });
            DropPrimaryKey("dbo.AspNetUsers");
            DropPrimaryKey("dbo.ChiefAdjudicators");
            DropPrimaryKey("dbo.OrganizationUsers");
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AspNetRoles",
                c => new {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            AddColumn("dbo.AspNetUsers", "EmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "PasswordHash", c => c.String());
            AddColumn("dbo.AspNetUsers", "SecurityStamp", c => c.String());
            AddColumn("dbo.AspNetUsers", "PhoneNumber", c => c.String());
            Sql("UPDATE dbo.AspNetUsers SET PhoneNumber=Phone");
            AddColumn("dbo.AspNetUsers", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "UserName", c => c.String(nullable: false, maxLength: 256));
            Sql("UPDATE dbo.AspNetUsers SET UserName=EMail");
            AlterColumn("dbo.Addresses", "CreatedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.Addresses", "ModifiedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(maxLength: 256));
            AlterColumn("dbo.ChiefAdjudicators", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Tournaments", "CreatedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.Tournaments", "ModifiedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.Organizations", "CreatedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.Organizations", "ModifiedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.TournamentOrganizationRegistrations", "CreatedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.TournamentOrganizationRegistrations", "ModifiedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.OrganizationUsers", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.OrganizationUsers", "CreatedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.OrganizationUsers", "ModifiedById", c => c.String(maxLength: 128));
            AlterColumn("dbo.OrganizationUsers", "User_Id", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.AspNetUsers", "Id");
            AddPrimaryKey("dbo.ChiefAdjudicators", new[] { "TournamentId", "UserId" });
            AddPrimaryKey("dbo.OrganizationUsers", new[] { "OrganizationId", "UserId" });
            CreateIndex("dbo.Addresses", "CreatedById");
            CreateIndex("dbo.Addresses", "ModifiedById");
            CreateIndex("dbo.AspNetUsers", "UserName", unique: true, name: "UserNameIndex");
            CreateIndex("dbo.ChiefAdjudicators", "UserId");
            CreateIndex("dbo.Tournaments", "CreatedById");
            CreateIndex("dbo.Tournaments", "ModifiedById");
            CreateIndex("dbo.Organizations", "CreatedById");
            CreateIndex("dbo.Organizations", "ModifiedById");
            CreateIndex("dbo.TournamentOrganizationRegistrations", "CreatedById");
            CreateIndex("dbo.TournamentOrganizationRegistrations", "ModifiedById");
            CreateIndex("dbo.OrganizationUsers", "UserId");
            CreateIndex("dbo.OrganizationUsers", "CreatedById");
            CreateIndex("dbo.OrganizationUsers", "ModifiedById");
            CreateIndex("dbo.OrganizationUsers", "User_Id");
            AddForeignKey("dbo.Tournaments", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Organizations", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Organizations", "ModifiedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TournamentOrganizationRegistrations", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.TournamentOrganizationRegistrations", "ModifiedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrganizationUsers", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrganizationUsers", "ModifiedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrganizationUsers", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Tournaments", "ModifiedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ChiefAdjudicators", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.OrganizationUsers", "User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Addresses", "CreatedById", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Addresses", "ModifiedById", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.AspNetUsers", "Phone");
            DropColumn("dbo.AspNetUsers", "Password");
            DropColumn("dbo.AspNetUsers", "UserRole");
            DropColumn("dbo.AspNetUsers", "CreatedById");
            DropColumn("dbo.AspNetUsers", "Created");
            DropColumn("dbo.AspNetUsers", "ModifiedById");
            DropColumn("dbo.AspNetUsers", "Modified");
        }

        public override void Down() {
            AddColumn("dbo.AspNetUsers", "Modified", c => c.DateTime());
            // TODO: Add Modified
            AddColumn("dbo.AspNetUsers", "ModifiedById", c => c.Guid(nullable: false));
            // TODO: Add ModifiedById
            AddColumn("dbo.AspNetUsers", "Created", c => c.DateTime());
            // TODO: Add Created
            AddColumn("dbo.AspNetUsers", "CreatedById", c => c.Guid(nullable: false));
            // TODO: Add CreatedById
            AddColumn("dbo.AspNetUsers", "UserRole", c => c.Int(nullable: false));
            // TODO: Fill UserRole
            AddColumn("dbo.AspNetUsers", "Password", c => c.String());
            // TODO: Generate passwords
            AddColumn("dbo.AspNetUsers", "Phone", c => c.String(maxLength: 16));
            // TODO: Copy phone numbers
            DropForeignKey("dbo.Addresses", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Addresses", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganizationUsers", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChiefAdjudicators", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tournaments", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganizationUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganizationUsers", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganizationUsers", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Organizations", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Organizations", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tournaments", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.OrganizationUsers", new[] { "User_Id" });
            DropIndex("dbo.OrganizationUsers", new[] { "ModifiedById" });
            DropIndex("dbo.OrganizationUsers", new[] { "CreatedById" });
            DropIndex("dbo.OrganizationUsers", new[] { "UserId" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "ModifiedById" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "CreatedById" });
            DropIndex("dbo.Organizations", new[] { "ModifiedById" });
            DropIndex("dbo.Organizations", new[] { "CreatedById" });
            DropIndex("dbo.Tournaments", new[] { "ModifiedById" });
            DropIndex("dbo.Tournaments", new[] { "CreatedById" });
            DropIndex("dbo.ChiefAdjudicators", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Addresses", new[] { "ModifiedById" });
            DropIndex("dbo.Addresses", new[] { "CreatedById" });
            DropPrimaryKey("dbo.OrganizationUsers");
            DropPrimaryKey("dbo.ChiefAdjudicators");
            DropPrimaryKey("dbo.AspNetUsers");
            AlterColumn("dbo.OrganizationUsers", "User_Id", c => c.Guid());
            AlterColumn("dbo.OrganizationUsers", "ModifiedById", c => c.Guid());
            AlterColumn("dbo.OrganizationUsers", "CreatedById", c => c.Guid());
            AlterColumn("dbo.OrganizationUsers", "UserId", c => c.Guid(nullable: false));
            AlterColumn("dbo.TournamentOrganizationRegistrations", "ModifiedById", c => c.Guid());
            AlterColumn("dbo.TournamentOrganizationRegistrations", "CreatedById", c => c.Guid());
            AlterColumn("dbo.Organizations", "ModifiedById", c => c.Guid());
            AlterColumn("dbo.Organizations", "CreatedById", c => c.Guid());
            AlterColumn("dbo.Tournaments", "ModifiedById", c => c.Guid());
            AlterColumn("dbo.Tournaments", "CreatedById", c => c.Guid());
            AlterColumn("dbo.ChiefAdjudicators", "UserId", c => c.Guid(nullable: false));
            AlterColumn("dbo.AspNetUsers", "Email", c => c.String(nullable: false, maxLength: 254));
            AlterColumn("dbo.AspNetUsers", "Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.Addresses", "ModifiedById", c => c.Guid());
            AlterColumn("dbo.Addresses", "CreatedById", c => c.Guid());
            DropColumn("dbo.AspNetUsers", "UserName");
            DropColumn("dbo.AspNetUsers", "AccessFailedCount");
            DropColumn("dbo.AspNetUsers", "LockoutEnabled");
            DropColumn("dbo.AspNetUsers", "LockoutEndDateUtc");
            DropColumn("dbo.AspNetUsers", "TwoFactorEnabled");
            DropColumn("dbo.AspNetUsers", "PhoneNumberConfirmed");
            DropColumn("dbo.AspNetUsers", "PhoneNumber");
            DropColumn("dbo.AspNetUsers", "SecurityStamp");
            DropColumn("dbo.AspNetUsers", "PasswordHash");
            DropColumn("dbo.AspNetUsers", "EmailConfirmed");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            AddPrimaryKey("dbo.OrganizationUsers", new[] { "OrganizationId", "UserId" });
            AddPrimaryKey("dbo.ChiefAdjudicators", new[] { "TournamentId", "UserId" });
            AddPrimaryKey("dbo.AspNetUsers", "Id");
            CreateIndex("dbo.OrganizationUsers", "User_Id");
            CreateIndex("dbo.OrganizationUsers", "ModifiedById");
            CreateIndex("dbo.OrganizationUsers", "CreatedById");
            CreateIndex("dbo.OrganizationUsers", "UserId");
            CreateIndex("dbo.TournamentOrganizationRegistrations", "ModifiedById");
            CreateIndex("dbo.TournamentOrganizationRegistrations", "CreatedById");
            CreateIndex("dbo.Organizations", "ModifiedById");
            CreateIndex("dbo.Organizations", "CreatedById");
            CreateIndex("dbo.Tournaments", "ModifiedById");
            CreateIndex("dbo.Tournaments", "CreatedById");
            CreateIndex("dbo.ChiefAdjudicators", "UserId");
            CreateIndex("dbo.AspNetUsers", "ModifiedById");
            CreateIndex("dbo.AspNetUsers", "CreatedById");
            CreateIndex("dbo.AspNetUsers", "EMail", unique: true);
            CreateIndex("dbo.Addresses", "ModifiedById");
            CreateIndex("dbo.Addresses", "CreatedById");
            AddForeignKey("dbo.Addresses", "ModifiedById", "dbo.Users", "Id");
            AddForeignKey("dbo.Addresses", "CreatedById", "dbo.Users", "Id");
            AddForeignKey("dbo.OrganizationUsers", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.ChiefAdjudicators", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.Tournaments", "ModifiedById", "dbo.Users", "Id");
            AddForeignKey("dbo.OrganizationUsers", "UserId", "dbo.Users", "Id");
            AddForeignKey("dbo.OrganizationUsers", "ModifiedById", "dbo.Users", "Id");
            AddForeignKey("dbo.OrganizationUsers", "CreatedById", "dbo.Users", "Id");
            AddForeignKey("dbo.TournamentOrganizationRegistrations", "ModifiedById", "dbo.Users", "Id");
            AddForeignKey("dbo.TournamentOrganizationRegistrations", "CreatedById", "dbo.Users", "Id");
            AddForeignKey("dbo.Organizations", "ModifiedById", "dbo.Users", "Id");
            AddForeignKey("dbo.Organizations", "CreatedById", "dbo.Users", "Id");
            AddForeignKey("dbo.Tournaments", "CreatedById", "dbo.Users", "Id");
            AddForeignKey("dbo.Users", "ModifiedById", "dbo.Users", "Id");
            AddForeignKey("dbo.Users", "CreatedById", "dbo.Users", "Id");
            RenameTable(name: "dbo.AspNetUsers", newName: "Users");
        }
    }
}
