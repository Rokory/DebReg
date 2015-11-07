delete from OrganizationUsers
delete from dbo.TournamentOrganizationRegistrations
delete from dbo.TournamentUserRoles
delete from dbo.AspNetRoles
delete from dbo.AspNetUserClaims
delete from dbo.AspNetUserLogins
delete from dbo.AspNetUserRoles
delete from dbo.AspNetUsers
delete from dbo.Tournaments

update dbo.Organizations SET LinkedOrganizationID = null
delete dbo.Organizations
delete dbo.Addresses
delete from dbo.SMTPHostConfigurations
