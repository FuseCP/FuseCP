-- This is to upgrade from SolidCP .net to FuseCP CoreWCF

-- Removing Helicon Zoo
-- GroupID: 42
-- ProviderID: 135

DELETE FROM [Quotas] WHERE [GroupID] = '42'
GO
DELETE FROM [ServiceDefaultProperties] WHERE [ProviderID] = '135'
GO
DELETE FROM [ServiceItemTypes] WHERE [GroupID] = '42'
GO
DELETE FROM [VirtualGroups] WHERE [GroupID] = '42' 
GO
DELETE FROM [dbo].[Services] WHERE [ProviderID] = '135'
GO
DELETE FROM [dbo].[Providers] WHERE [ProviderID] = '135'
GO
DELETE FROM [dbo].[ResourceGroups] WHERE [GroupID] = '42'
GO


-- Removing Microsoft Web Platform Installer (WebPI)
DELETE FROM [dbo].[SystemSettings] WHERE [PropertyName] = 'WpiSettings'
GO

-- Removing Windows Server 2003
-- ProviderID: 1

IF EXISTS (SELECT * FROM [dbo].[Services] WHERE [ProviderID] = '1')
BEGIN
	UPDATE [Services] SET [ProviderID]='111' WHERE [ProviderID] = '1'
END
GO

IF EXISTS (SELECT * FROM [dbo].[Providers] WHERE [ProviderID] = '1' AND DisplayName = 'Windows Server 2003')
BEGIN
DELETE FROM [dbo].[ServiceDefaultProperties] WHERE [ProviderID] = '1'
DELETE FROM [dbo].[Providers] WHERE [ProviderID] = '1' AND [DisplayName] = 'Windows Server 2003'
END
GO

-- Removing Windows Server 2008
-- ProviderID: 100

IF EXISTS (SELECT * FROM [dbo].[Services] WHERE [ProviderID] = '100')
BEGIN
	UPDATE [Services] SET [ProviderID]='111' WHERE [ProviderID] = '100'
END
GO

IF EXISTS (SELECT * FROM [dbo].[Providers] WHERE [ProviderID] = '100' AND [DisplayName] = 'Windows Server 2008')
BEGIN
DELETE FROM [dbo].[ServiceDefaultProperties] WHERE [ProviderID] = '100'
DELETE FROM [dbo].[Providers] WHERE [ProviderID] = '100' AND [DisplayName] = 'Windows Server 2008'
END
GO

-- Removing Windows Server 2012
-- ProviderID: 104

IF EXISTS (SELECT * FROM [dbo].[Services] WHERE [ProviderID] = '104')
BEGIN
	UPDATE [Services] SET [ProviderID]='111' WHERE [ProviderID] = '104'
END
GO

IF EXISTS (SELECT * FROM [dbo].[Providers] WHERE [ProviderID] = '104' AND [DisplayName] = 'Windows Server 2012')
BEGIN
DELETE FROM [dbo].[ServiceDefaultProperties] WHERE [ProviderID] = '104'
DELETE FROM [dbo].[Providers] WHERE [ProviderID] = '104' AND [DisplayName] = 'Windows Server 2012'
END
GO