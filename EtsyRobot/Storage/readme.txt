Install http://www.microsoft.com/en-us/download/confirmation.aspx?id=42295

1. Install PM packages:
	install-Package Npgsql.EF6 -Pre
	install-package Npgsql.EntityFramework -pre
2. Manual db migration
   Add-Migration
   Update-Database -Verbose

PM> Enable-Migrations -EnableAutomaticMigrations -Force -ContextTypeName EtsyRobot.Storage.Infrastructure.CoreContext
Add-Migration
PM> Update-Database