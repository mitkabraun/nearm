select
physical_name, size * 8 / 1024
from sys.master_files
where type = 0 and database_id = @Id
