CREATE TABLE [dbo].[tbDetection]
(
	[DTC_Id] INT NOT NULL PRIMARY KEY, 
    [CTM_Id] INT NOT NULL, 
    [ITM_Id] INT NOT NULL, 
    [CPT_Id] INT NOT NULL, 
    [DTC_Probability] FLOAT NULL, 
    [DTC_PositionX] FLOAT NULL, 
    [DTC_PositionY] FLOAT NULL, 
    [DTC_Width] FLOAT NULL, 
    [DTC_Height] FLOAT NULL,
    CONSTRAINT FK_tbCriticalMoment FOREIGN KEY (CTM_Id) REFERENCES tbCriticalMoment(CTM_Id),
    CONSTRAINT FK_tbInstrument FOREIGN KEY (ITM_Id) REFERENCES tbInstrument(ITM_Id),
    CONSTRAINT FK_tbCapture FOREIGN KEY (CPT_Id) REFERENCES tbCapture(CPT_Id),
)
