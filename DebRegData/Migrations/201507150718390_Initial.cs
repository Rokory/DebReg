namespace DebReg.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StreetAddress1 = c.String(maxLength: 70),
                        StreetAddress2 = c.String(maxLength: 70),
                        PostalCode = c.String(maxLength: 9),
                        City = c.String(maxLength: 70),
                        Region = c.String(maxLength: 70),
                        Country = c.String(maxLength: 70),
                        CountryId = c.Guid(),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .Index(t => t.CountryId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ShortName = c.String(),
                        Alpha2 = c.String(maxLength: 2),
                        Alpha3 = c.String(maxLength: 3),
                        NumericCode = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 70),
                        LastName = c.String(nullable: false, maxLength: 70),
                        Email = c.String(nullable: false, maxLength: 256),
                        NewEMail = c.String(maxLength: 255),
                        PhoneNumber = c.String(maxLength: 16),
                        SponsoringOrganizationId = c.Guid(nullable: false),
                        CurrentOrganizationId = c.Guid(),
                        CurrentTournamentId = c.Guid(),
                        PasswordChangeRequired = c.Boolean(nullable: false),
                        LastSMTPErrorHostConfigurationId = c.Guid(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.CurrentOrganizationId)
                .ForeignKey("dbo.Tournaments", t => t.CurrentTournamentId)
                .ForeignKey("dbo.SMTPHostConfigurations", t => t.LastSMTPErrorHostConfigurationId)
                .ForeignKey("dbo.Organizations", t => t.SponsoringOrganizationId)
                .Index(t => t.SponsoringOrganizationId)
                .Index(t => t.CurrentOrganizationId)
                .Index(t => t.CurrentTournamentId)
                .Index(t => t.LastSMTPErrorHostConfigurationId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Adjudicators",
                c => new
                    {
                        TournamentId = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        OrganizationId = c.Guid(),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.TournamentId, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TournamentId)
                .Index(t => t.UserId)
                .Index(t => t.OrganizationId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 70),
                        Abbreviation = c.String(),
                        University = c.Boolean(nullable: false),
                        LinkedOrganizationId = c.Guid(),
                        AddressId = c.Guid(),
                        VatId = c.String(maxLength: 14),
                        Status = c.Int(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                        SMTPHostConfiguration_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Organizations", t => t.LinkedOrganizationId)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.SMTPHostConfigurations", t => t.SMTPHostConfiguration_Id)
                .Index(t => t.LinkedOrganizationId)
                .Index(t => t.AddressId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById)
                .Index(t => t.SMTPHostConfiguration_Id);
            
            CreateTable(
                "dbo.SMTPHostConfigurations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Host = c.String(maxLength: 255),
                        Port = c.Int(nullable: false),
                        SSL = c.Boolean(nullable: false),
                        Username = c.String(maxLength: 255),
                        EncryptedPassword = c.Binary(),
                        FromAddress = c.String(maxLength: 255),
                        LastSMTPOperationDateTime = c.DateTime(),
                        LastSMTPError = c.String(maxLength: 1500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TournamentOrganizationRegistrations",
                c => new
                    {
                        TournamentId = c.Guid(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                        BookingCode = c.String(maxLength: 6),
                        BilledOrganizationId = c.Guid(nullable: false),
                        TeamsWanted = c.Int(nullable: false),
                        AdjudicatorsWanted = c.Int(nullable: false),
                        TeamsGranted = c.Int(nullable: false),
                        AdjudicatorsGranted = c.Int(nullable: false),
                        TeamsPaid = c.Int(nullable: false),
                        AdjudicatorsPaid = c.Int(nullable: false),
                        Notes = c.String(unicode: false, storeType: "text"),
                        OrganizationStatus = c.Int(nullable: false),
                        OrganizationStatusDraft = c.Boolean(nullable: false),
                        OrganizationStatusNote = c.String(unicode: false, storeType: "text"),
                        Rank = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RandomRank = c.Int(nullable: false),
                        LockAutoAssign = c.Boolean(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                        Organization_Id = c.Guid(),
                    })
                .PrimaryKey(t => new { t.TournamentId, t.OrganizationId })
                .ForeignKey("dbo.Organizations", t => t.BilledOrganizationId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .ForeignKey("dbo.Organizations", t => t.Organization_Id)
                .Index(t => t.TournamentId)
                .Index(t => t.OrganizationId)
                .Index(t => t.BookingCode)
                .Index(t => t.BilledOrganizationId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById)
                .Index(t => t.Organization_Id);
            
            CreateTable(
                "dbo.Tournaments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        HostingOrganizationID = c.Guid(nullable: false),
                        Name = c.String(maxLength: 70),
                        Location = c.String(maxLength: 70),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        RegistrationStart = c.DateTime(),
                        RegistrationEnd = c.DateTime(),
                        AdjucatorSubtract = c.Int(),
                        TeamSize = c.Int(nullable: false),
                        TeamCap = c.Int(nullable: false),
                        AdjudicatorCap = c.Int(nullable: false),
                        TeamProductId = c.Guid(),
                        AdjudicatorProductId = c.Guid(),
                        UniversityRequired = c.Boolean(nullable: false),
                        CurrencyId = c.Guid(),
                        FinanceEMail = c.String(),
                        TermsConditions = c.String(maxLength: 1500),
                        TermsConditionsLink = c.String(maxLength: 1500),
                        MoneyTransferLinkCaption = c.String(maxLength: 1500),
                        MoneyTransferLink = c.String(maxLength: 1500),
                        PaymentReference = c.String(maxLength: 255),
                        BankAccountId = c.Guid(),
                        FixedTeamNames = c.Boolean(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.AdjudicatorProductId)
                .ForeignKey("dbo.BankAccounts", t => t.BankAccountId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.Currencies", t => t.CurrencyId)
                .ForeignKey("dbo.Organizations", t => t.HostingOrganizationID)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Products", t => t.TeamProductId)
                .Index(t => t.HostingOrganizationID)
                .Index(t => t.TeamProductId)
                .Index(t => t.AdjudicatorProductId)
                .Index(t => t.CurrencyId)
                .Index(t => t.BankAccountId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TournamentId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 70),
                        Description = c.String(maxLength: 1500),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VatRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                        Tournament_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .ForeignKey("dbo.Tournaments", t => t.Tournament_Id)
                .Index(t => t.TournamentId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById)
                .Index(t => t.Tournament_Id);
            
            CreateTable(
                "dbo.BankAccounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                        BankName = c.String(),
                        BankAddressId = c.Guid(),
                        Iban = c.String(maxLength: 34),
                        Bic = c.String(),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Addresses", t => t.BankAddressId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.OrganizationId)
                .Index(t => t.BankAddressId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Symbol = c.String(),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TournamentId = c.Guid(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                        Name = c.String(),
                        AutoSuffix = c.String(),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .Index(t => t.TournamentId)
                .Index(t => t.OrganizationId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.TournamentUserRoles",
                c => new
                    {
                        TournamentId = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TournamentId, t.UserId, t.Role })
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.TournamentId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.OrganizationUsers",
                c => new
                    {
                        OrganizationId = c.Guid(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Role = c.Int(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganizationId, t.UserId, t.Role })
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .Index(t => t.OrganizationId)
                .Index(t => t.UserId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
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
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserPropertyValues",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        UserPropertyId = c.Guid(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => new { t.UserId, t.UserPropertyId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.UserProperties", t => t.UserPropertyId)
                .Index(t => t.UserId)
                .Index(t => t.UserPropertyId);
            
            CreateTable(
                "dbo.UserProperties",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Type = c.Int(nullable: false),
                        Min = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Max = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Required = c.Boolean(nullable: false),
                        TournamentSpecific = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPropertyOptions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Order = c.Int(nullable: false),
                        UserPropertyId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProperties", t => t.UserPropertyId)
                .Index(t => t.UserPropertyId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.UserTournamentPropertyValues",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        UserPropertyId = c.Guid(nullable: false),
                        TournamentId = c.Guid(nullable: false),
                        Value = c.String(),
                    })
                .PrimaryKey(t => new { t.UserId, t.UserPropertyId, t.TournamentId })
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.UserProperties", t => t.UserPropertyId)
                .Index(t => t.UserId)
                .Index(t => t.UserPropertyId)
                .Index(t => t.TournamentId);
            
            CreateTable(
                "dbo.BookingRecords",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                        TournamentId = c.Guid(nullable: false),
                        ProductId = c.Guid(),
                        ProductName = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VatRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Credit = c.Boolean(nullable: false),
                        PaymentsDueDate = c.DateTime(),
                        Note = c.String(maxLength: 1500),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .Index(t => t.OrganizationId)
                .Index(t => t.TournamentId)
                .Index(t => t.ProductId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.SlotAssignments",
                c => new
                    {
                        TournamentId = c.Guid(nullable: false),
                        OrganizationId = c.Guid(nullable: false),
                        VersionId = c.Guid(nullable: false),
                        TeamsGranted = c.Int(nullable: false),
                        AdjucatorsGranted = c.Int(nullable: false),
                        CreatedById = c.String(maxLength: 128),
                        Created = c.DateTime(),
                        ModifiedById = c.String(maxLength: 128),
                        Modified = c.DateTime(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.TournamentId, t.OrganizationId, t.VersionId })
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById)
                .ForeignKey("dbo.AspNetUsers", t => t.ModifiedById)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .ForeignKey("dbo.Versions", t => t.VersionId)
                .Index(t => t.TournamentId)
                .Index(t => t.OrganizationId)
                .Index(t => t.VersionId)
                .Index(t => t.CreatedById)
                .Index(t => t.ModifiedById);
            
            CreateTable(
                "dbo.Versions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Number = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserTournamentProperties",
                c => new
                    {
                        UserPropertyId = c.Guid(nullable: false),
                        TournamentId = c.Guid(nullable: false),
                        Required = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserPropertyId, t.TournamentId })
                .ForeignKey("dbo.Tournaments", t => t.TournamentId)
                .ForeignKey("dbo.UserProperties", t => t.UserPropertyId)
                .Index(t => t.UserPropertyId)
                .Index(t => t.TournamentId);
            
            CreateTable(
                "dbo.TeamSpeaker",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        TeamId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.TeamId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.TeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTournamentProperties", "UserPropertyId", "dbo.UserProperties");
            DropForeignKey("dbo.UserTournamentProperties", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.SlotAssignments", "VersionId", "dbo.Versions");
            DropForeignKey("dbo.SlotAssignments", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.SlotAssignments", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.SlotAssignments", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.SlotAssignments", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.BookingRecords", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.BookingRecords", "ProductId", "dbo.Products");
            DropForeignKey("dbo.BookingRecords", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.BookingRecords", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.BookingRecords", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Addresses", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Addresses", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserTournamentPropertyValues", "UserPropertyId", "dbo.UserProperties");
            DropForeignKey("dbo.UserTournamentPropertyValues", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserTournamentPropertyValues", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TeamSpeaker", "TeamId", "dbo.Teams");
            DropForeignKey("dbo.TeamSpeaker", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "SponsoringOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPropertyValues", "UserPropertyId", "dbo.UserProperties");
            DropForeignKey("dbo.UserPropertyOptions", "UserPropertyId", "dbo.UserProperties");
            DropForeignKey("dbo.UserPropertyValues", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "LastSMTPErrorHostConfigurationId", "dbo.SMTPHostConfigurations");
            DropForeignKey("dbo.AspNetUsers", "CurrentTournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.AspNetUsers", "CurrentOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Adjudicators", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Adjudicators", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.Adjudicators", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationUsers", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationUsers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganizationUsers", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganizationUsers", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "Organization_Id", "dbo.Organizations");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.TournamentUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TournamentUserRoles", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.Teams", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.Teams", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Teams", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Teams", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tournaments", "TeamProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "Tournament_Id", "dbo.Tournaments");
            DropForeignKey("dbo.Tournaments", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tournaments", "HostingOrganizationID", "dbo.Organizations");
            DropForeignKey("dbo.Tournaments", "CurrencyId", "dbo.Currencies");
            DropForeignKey("dbo.Currencies", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Currencies", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tournaments", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Tournaments", "BankAccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.BankAccounts", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.BankAccounts", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.BankAccounts", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.BankAccounts", "BankAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Tournaments", "AdjudicatorProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "TournamentId", "dbo.Tournaments");
            DropForeignKey("dbo.Products", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Products", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.TournamentOrganizationRegistrations", "BilledOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Organizations", "SMTPHostConfiguration_Id", "dbo.SMTPHostConfigurations");
            DropForeignKey("dbo.Organizations", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Organizations", "LinkedOrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Organizations", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Organizations", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Adjudicators", "ModifiedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Adjudicators", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Addresses", "CountryId", "dbo.Countries");
            DropIndex("dbo.TeamSpeaker", new[] { "TeamId" });
            DropIndex("dbo.TeamSpeaker", new[] { "UserId" });
            DropIndex("dbo.UserTournamentProperties", new[] { "TournamentId" });
            DropIndex("dbo.UserTournamentProperties", new[] { "UserPropertyId" });
            DropIndex("dbo.SlotAssignments", new[] { "ModifiedById" });
            DropIndex("dbo.SlotAssignments", new[] { "CreatedById" });
            DropIndex("dbo.SlotAssignments", new[] { "VersionId" });
            DropIndex("dbo.SlotAssignments", new[] { "OrganizationId" });
            DropIndex("dbo.SlotAssignments", new[] { "TournamentId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.BookingRecords", new[] { "ModifiedById" });
            DropIndex("dbo.BookingRecords", new[] { "CreatedById" });
            DropIndex("dbo.BookingRecords", new[] { "ProductId" });
            DropIndex("dbo.BookingRecords", new[] { "TournamentId" });
            DropIndex("dbo.BookingRecords", new[] { "OrganizationId" });
            DropIndex("dbo.UserTournamentPropertyValues", new[] { "TournamentId" });
            DropIndex("dbo.UserTournamentPropertyValues", new[] { "UserPropertyId" });
            DropIndex("dbo.UserTournamentPropertyValues", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.UserPropertyOptions", new[] { "UserPropertyId" });
            DropIndex("dbo.UserPropertyValues", new[] { "UserPropertyId" });
            DropIndex("dbo.UserPropertyValues", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.OrganizationUsers", new[] { "ModifiedById" });
            DropIndex("dbo.OrganizationUsers", new[] { "CreatedById" });
            DropIndex("dbo.OrganizationUsers", new[] { "UserId" });
            DropIndex("dbo.OrganizationUsers", new[] { "OrganizationId" });
            DropIndex("dbo.TournamentUserRoles", new[] { "UserId" });
            DropIndex("dbo.TournamentUserRoles", new[] { "TournamentId" });
            DropIndex("dbo.Teams", new[] { "ModifiedById" });
            DropIndex("dbo.Teams", new[] { "CreatedById" });
            DropIndex("dbo.Teams", new[] { "OrganizationId" });
            DropIndex("dbo.Teams", new[] { "TournamentId" });
            DropIndex("dbo.Currencies", new[] { "ModifiedById" });
            DropIndex("dbo.Currencies", new[] { "CreatedById" });
            DropIndex("dbo.BankAccounts", new[] { "ModifiedById" });
            DropIndex("dbo.BankAccounts", new[] { "CreatedById" });
            DropIndex("dbo.BankAccounts", new[] { "BankAddressId" });
            DropIndex("dbo.BankAccounts", new[] { "OrganizationId" });
            DropIndex("dbo.Products", new[] { "Tournament_Id" });
            DropIndex("dbo.Products", new[] { "ModifiedById" });
            DropIndex("dbo.Products", new[] { "CreatedById" });
            DropIndex("dbo.Products", new[] { "TournamentId" });
            DropIndex("dbo.Tournaments", new[] { "ModifiedById" });
            DropIndex("dbo.Tournaments", new[] { "CreatedById" });
            DropIndex("dbo.Tournaments", new[] { "BankAccountId" });
            DropIndex("dbo.Tournaments", new[] { "CurrencyId" });
            DropIndex("dbo.Tournaments", new[] { "AdjudicatorProductId" });
            DropIndex("dbo.Tournaments", new[] { "TeamProductId" });
            DropIndex("dbo.Tournaments", new[] { "HostingOrganizationID" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "Organization_Id" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "ModifiedById" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "CreatedById" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "BilledOrganizationId" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "BookingCode" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "OrganizationId" });
            DropIndex("dbo.TournamentOrganizationRegistrations", new[] { "TournamentId" });
            DropIndex("dbo.Organizations", new[] { "SMTPHostConfiguration_Id" });
            DropIndex("dbo.Organizations", new[] { "ModifiedById" });
            DropIndex("dbo.Organizations", new[] { "CreatedById" });
            DropIndex("dbo.Organizations", new[] { "AddressId" });
            DropIndex("dbo.Organizations", new[] { "LinkedOrganizationId" });
            DropIndex("dbo.Adjudicators", new[] { "ModifiedById" });
            DropIndex("dbo.Adjudicators", new[] { "CreatedById" });
            DropIndex("dbo.Adjudicators", new[] { "OrganizationId" });
            DropIndex("dbo.Adjudicators", new[] { "UserId" });
            DropIndex("dbo.Adjudicators", new[] { "TournamentId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "LastSMTPErrorHostConfigurationId" });
            DropIndex("dbo.AspNetUsers", new[] { "CurrentTournamentId" });
            DropIndex("dbo.AspNetUsers", new[] { "CurrentOrganizationId" });
            DropIndex("dbo.AspNetUsers", new[] { "SponsoringOrganizationId" });
            DropIndex("dbo.Addresses", new[] { "ModifiedById" });
            DropIndex("dbo.Addresses", new[] { "CreatedById" });
            DropIndex("dbo.Addresses", new[] { "CountryId" });
            DropTable("dbo.TeamSpeaker");
            DropTable("dbo.UserTournamentProperties");
            DropTable("dbo.Versions");
            DropTable("dbo.SlotAssignments");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.BookingRecords");
            DropTable("dbo.UserTournamentPropertyValues");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.UserPropertyOptions");
            DropTable("dbo.UserProperties");
            DropTable("dbo.UserPropertyValues");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.OrganizationUsers");
            DropTable("dbo.TournamentUserRoles");
            DropTable("dbo.Teams");
            DropTable("dbo.Currencies");
            DropTable("dbo.BankAccounts");
            DropTable("dbo.Products");
            DropTable("dbo.Tournaments");
            DropTable("dbo.TournamentOrganizationRegistrations");
            DropTable("dbo.SMTPHostConfigurations");
            DropTable("dbo.Organizations");
            DropTable("dbo.Adjudicators");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Countries");
            DropTable("dbo.Addresses");
        }
    }
}
