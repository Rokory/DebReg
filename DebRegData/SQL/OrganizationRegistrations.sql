select o.Name, o.Abbreviation, u.Email, r.TeamsWanted, r.AdjudicatorsWanted, r.Notes
from dbo.AspNetUsers u 
inner join dbo.Organizations o 
on u.CurrentOrganizationId = o.Id 
inner join dbo.TournamentOrganizationRegistrations r 
on o.Id = r.OrganizationId 