select name
from sys.databases
where name not in ('master', 'tempdb', 'model', 'msdb')
order by name
