CREATE TABLE [dbo].[tbCriticalCapture]
(
	[CTC_Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SGR_Id] INT NOT NULL, 
    [CTC_EventDateTime] DATE NULL, 
    [CTC_EventTime] INT NULL,
    [CTC_CaptureName] VARCHAR(200) NULL, 
    [CTC_CapturePath] VARCHAR(200) NULL, 
    CONSTRAINT FK_tbSurgery FOREIGN KEY (SGR_Id) REFERENCES tbSurgery(SGR_Id)
)
