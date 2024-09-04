CREATE TABLE [dbo].[tbCriticalMoment]
(
	[CTM_Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SGR_Id] INT NOT NULL, 
    [CTM_EventDateTime] DATE NULL, 
    [CTM_EventTime] INT NULL,
    CONSTRAINT FK_tbSurgery FOREIGN KEY (SGR_Id) REFERENCES tbSurgery(SGR_Id)
)
