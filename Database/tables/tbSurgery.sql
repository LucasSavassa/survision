CREATE TABLE [dbo].[tbSurgery]
(
	[SGR_Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [SGR_StartTime] DATETIME NULL, 
    [SGR_Identification] VARCHAR(200) NULL, 
    [SGR_Duration] INT NULL, 
    [SGR_PlannedStartTime] DATETIME NULL, 
    [SGR_SurgeryRoom] VARCHAR(200) NULL, 
    [SGR_PatientName] VARCHAR(200) NULL, 
    [SGR_SurgeryType] VARCHAR(200) NULL, 
    [SGR_SurgeryStatus] INT NULL
)
