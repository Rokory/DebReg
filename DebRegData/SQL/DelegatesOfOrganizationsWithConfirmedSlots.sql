select u.FirstName, u.LastName, u.Email, o.Name 'Organization Name', r.TeamsPaid, r.AdjudicatorsPaid
from AspNetUsers u
inner join OrganizationUsers ou
on u.Id = ou.UserId
inner join Organizations o
on o.Id = ou.OrganizationId
inner join TournamentOrganizationRegistrations r
on r.OrganizationId = o.Id
where r.AdjudicatorsPaid > 0
or r.TeamsPaid > 0
and ou.Role=2
