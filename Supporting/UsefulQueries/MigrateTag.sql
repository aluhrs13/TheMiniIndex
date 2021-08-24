DECLARE @tagToDelete int
SET @tagToDelete='4494'

DECLARE @tagToMigrateTo int
SET @tagToMigrateTo='5485'

UPDATE dbo.MiniTag SET TagID=@tagToMigrateTo WHERE TagID=@tagToDelete;
DELETE FROM dbo.Tag WHERE ID=@tagToDelete;