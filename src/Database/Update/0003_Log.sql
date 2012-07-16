CREATE TABLE [dbo].[Log] (
    [Id] [int] IDENTITY (1, 1) NOT NULL,
    [Date] [datetime] NOT NULL,
    [Level] [varchar] (50) NOT NULL,
    [Logger] [varchar] (255) NOT NULL,
    [Message] [varchar] (5000) NOT NULL,
    [Subject] [varchar] (200) NOT NULL,
    [Exception] [varchar] (2000) NULL
)