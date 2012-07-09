CREATE TABLE dbo.ItemRecords(
	Id bigint IDENTITY(1,1) NOT NULL,
	ItemName varchar(300) not null,
	AssemblyName varchar(300) null,
	ClassName varchar(300) not null,
	ItemAttributes varchar(3000) not null,
	IsBad bit not null default 0,
	CreatedDate datetime NULL
) ON [PRIMARY]

GO

ALTER TABLE dbo.ItemRecords ADD  DEFAULT (getdate()) FOR CreatedDate
GO
