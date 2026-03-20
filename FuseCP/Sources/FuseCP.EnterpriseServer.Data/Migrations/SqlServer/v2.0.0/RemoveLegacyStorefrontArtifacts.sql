IF EXISTS (SELECT 1 FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ServiceHandlersResponsesDetailed]'))
DROP VIEW [dbo].[ServiceHandlersResponsesDetailed]
GO
IF EXISTS (SELECT 1 FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ContractsServicesDetailed]'))
DROP VIEW [dbo].[ContractsServicesDetailed]
GO
IF EXISTS (SELECT 1 FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[ContractsInvoicesDetailed]'))
DROP VIEW [dbo].[ContractsInvoicesDetailed]
GO

DECLARE @dropProcedures nvarchar(max) = N'';

SELECT @dropProcedures = @dropProcedures +
	N'DROP PROCEDURE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + N';' + CHAR(13) + CHAR(10)
FROM sys.procedures
WHERE name LIKE N'ec%';

IF LEN(@dropProcedures) > 0
	EXEC sp_executesql @dropProcedures;
GO

DECLARE @dropConstraints nvarchar(max) = N'';

SELECT @dropConstraints = @dropConstraints +
	N'ALTER TABLE ' + QUOTENAME(SCHEMA_NAME(parentTable.schema_id)) + N'.' + QUOTENAME(parentTable.name) +
	N' DROP CONSTRAINT ' + QUOTENAME(foreignKey.name) + N';' + CHAR(13) + CHAR(10)
FROM sys.foreign_keys foreignKey
INNER JOIN sys.tables parentTable ON parentTable.object_id = foreignKey.parent_object_id
WHERE parentTable.name LIKE N'ec%';

IF LEN(@dropConstraints) > 0
	EXEC sp_executesql @dropConstraints;
GO

DECLARE @dropTables nvarchar(max) = N'';

SELECT @dropTables = @dropTables +
	N'DROP TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + N'.' + QUOTENAME(name) + N';' + CHAR(13) + CHAR(10)
FROM sys.tables
WHERE name LIKE N'ec%';

IF LEN(@dropTables) > 0
	EXEC sp_executesql @dropTables;
GO