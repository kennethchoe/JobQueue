CREATE TABLE dbo.SqlQueueLogs(
	Id bigint IDENTITY(1,1) NOT NULL,
	LogText [varchar](2000) NULL,
	ErrorDetails varchar(6000) NULL,
	CreatedDate datetime NULL
) ON [PRIMARY]

GO

ALTER TABLE dbo.SqlQueueLogs ADD  DEFAULT (getdate()) FOR CreatedDate
GO
