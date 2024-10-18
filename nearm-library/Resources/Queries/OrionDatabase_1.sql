select
database_id, create_date
from sys.databases
where name = @Name
