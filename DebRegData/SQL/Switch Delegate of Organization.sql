declare @oldDelegateEmail varchar(max)
declare @newDelgateEmail varchar(max)
declare @organizationName varchar(max)

set @oldDelegateEmail = 'l.parchment@lancaster.ac.uk'
set @newDelgateEmail = 'k.hatter@lancaster.ac.uk'
set @organizationName = 'Lancaster Debating Union'

insert into OrganizationUsers
(UserId, OrganizationId, Role)
select u.Id, o.Id, 2
from AspNetUsers u, Organizations o
where u.Email = @newDelgateEmail
and o.Name = @organizationName

update OrganizationUsers
set Role = 1
where UserId = (
	select Id
	from AspNetUsers
	where Email = @oldDelegateEmail
)
and OrganizationId = (
	select Id
	from Organizations
	where Name = @organizationName
)
