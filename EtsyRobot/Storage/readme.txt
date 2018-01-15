Install http://www.microsoft.com/en-us/download/confirmation.aspx?id=42295

1. Install PM packages:
	install-Package Npgsql.EF6 -Pre
	install-package Npgsql.EntityFramework -pre
2. Manual db migration
   Enable-Migrations -EnableAutomaticMigrations -Force  //-ContextTypeName CoreEntities,  Softengi.PrivDogAutomation.Storage.Infrastructure.CoreContext
   Add-Migration
   Update-Database -Verbose