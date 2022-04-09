set /P DB_NAME=LocalDB Database Name:
set MDF_PATH=%cd%
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE [%DB_NAME%] ON PRIMARY ( NAME=[%DB_NAME%_data], FILENAME = '%MDF_PATH%\%DB_NAME%_data.mdf') LOG ON (NAME=[%DB_NAME%_log], FILENAME = '%MDF_PATH%\%DB_NAME%_log.ldf');"