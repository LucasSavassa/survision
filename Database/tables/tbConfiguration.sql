CREATE TABLE [dbo].[tbConfiguration]
(
	[CFG_Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CFG_CaptureFolder] VARCHAR(200) NULL, 
    [CFG_QueueFolder] VARCHAR(200) NULL, 
    [CFG_ProcessingFolder] VARCHAR(200) NULL, 
    [CFG_ProcessedFolder] VARCHAR(200) NULL, 
    [CFG_SaveAllCaptures] BIT NULL, 
    [CFG_BackupFolder] VARCHAR(200) NULL, 
    [CFG_CaptureInterval] INT NULL
)
