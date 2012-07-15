CREATE TABLE [dbo].[TestLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[LogDate] [datetime] NULL,
	[LogText] [varchar](8000) NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TestLogs] ADD  DEFAULT (getdate()) FOR [LogDate]
GO
