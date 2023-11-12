-- QueryCommands.sql

-- CreateAppTableQuery
CREATE TABLE IF NOT EXISTS App (ID INTEGER PRIMARY KEY, Name TEXT NOT NULL);
-- END

-- CreateUsageTableQuery
CREATE TABLE IF NOT EXISTS Usage (ID INTEGER, YY INTEGER, MM INTEGER, DD INTEGER, minutes INTEGER, FOREIGN KEY(ID) REFERENCES App(ID));
-- END

-- AddApp
INSERT INTO App (ID, Name) VALUES (@id, @name);
-- END

-- DeleteApp
DELETE FROM App WHERE ID = @id;
-- END

-- GetAppUsageByDay
SELECT minutes FROM Usage WHERE ID = @id AND YY = @year AND MM = @month AND DD = @day;
-- END

-- GetAppUsageByMonth
SELECT SUM(minutes) AS TotalMinutes FROM Usage WHERE ID = @id AND YY = @year AND MM = @month GROUP BY ID;
-- END

-- UpdateAppUsage
UPDATE Usage SET minutes = @minutes WHERE ID = @id AND YY = @year AND MM = @month AND DD = @day;
-- END

-- InsertAppUsage
INSERT INTO Usage (ID, YY, MM, DD, minutes) VALUES (@id, @year, @month, @day, @minutes);
-- END

-- GetAllApps
SELECT DISTINCT Name FROM App;
-- END

