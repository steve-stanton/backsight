USE [Backsight]
GO

-- Get rid of the data for a job

DECLARE @JobId int;
SELECT @JobId=JobId FROM ced.Jobs WHERE [Name]='Selkirk';

-- When you delete from the Sessions table, the database will also delete
-- rows from the ced.Edits table.

DELETE FROM ced.Sessions WHERE JobId = @JobId;

-- If you want to also trash the job definition (i.e. you don't want to re-use
-- the job, do the following as well).

DELETE FROM ced.IdAllocations WHERE JobId = @JobId;
DELETE FROM ced.UserJobs WHERE JobId = @JobId;
DELETE FROM ced.Jobs WHERE JobId = @JobId;

GO

