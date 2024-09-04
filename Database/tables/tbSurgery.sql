CREATE TABLE [dbo].[tbSurgery]
(
	[SGR_Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SGR_StartTime] DATETIME NULL, 
    [SGR_Identification] VARCHAR(200) NULL, 
    [SGR_Duration] INT NULL
)
