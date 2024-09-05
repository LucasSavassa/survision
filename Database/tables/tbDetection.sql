CREATE TABLE [dbo].[tbDetection]
(
	[DTC_Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CTC_Id] INT NOT NULL, 
    [ITM_Id] INT NOT NULL, 
    [DTC_Probability] FLOAT NULL, 
    [DTC_PositionX] FLOAT NULL, 
    [DTC_PositionY] FLOAT NULL, 
    [DTC_Width] FLOAT NULL, 
    [DTC_Height] FLOAT NULL,
    CONSTRAINT FK_tbCriticalMoment FOREIGN KEY ([CTC_Id]) REFERENCES tbCriticalCapture([CTC_Id]),
    CONSTRAINT FK_tbInstrument FOREIGN KEY (ITM_Id) REFERENCES tbInstrument(ITM_Id),
)
