CREATE TABLE [dbo].[SyncDBLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LogDate] [datetime] NULL,
	[LogText] [varchar](8000) NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SyncDBLogs] ADD  DEFAULT (getdate()) FOR [LogDate]
GO
