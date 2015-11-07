declare @userEMail varchar(max)

set @userEMail = 'l.parchment@lancaster.ac.uk'

select u.Email, o.Name, ou.Role
from AspNetUsers u
inner join OrganizationUsers ou
on ou.UserId = u.Id
inner join Organizations o
on o.Id = ou.OrganizationId
where Email = @userEMail