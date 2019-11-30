DECLARE @tagToDelete int
SET @tagToDelete='4124'

DELETE FROM dbo.MiniTag WHERE TagID=@tagToDelete;
DELETE FROM dbo.Tag WHERE ID=@tagToDelete;