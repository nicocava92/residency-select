﻿CREATE TABLE [dbo].[Manual_Hogan_Import] (
    [Id]                                   SMALLINT     IDENTITY (1, 1) NOT NULL,
    [ClientID]                             VARCHAR (50) NULL,
    [ClientName]                           VARCHAR (50) NULL,
    [GroupName]                            VARCHAR (50) NULL,
    [Hogan_User_ID]                        VARCHAR (50) NOT NULL,
    [First_Name]                           VARCHAR (50) NULL,
    [Last_Name]                            VARCHAR (50) NULL,
    [Gender]                               VARCHAR (50) NULL,
    [HPIDate]                              VARCHAR (50) NOT NULL,
    [Valid]                                INT          NULL,
    [Empathy]                              INT          NULL,
    [NotAnxious]                           INT          NULL,
    [NoGuilt]                              INT          NULL,
    [Calmness]                             INT          NULL,
    [EvenTempered]                         INT          NULL,
    [NoSomaticComplaint]                   INT          NULL,
    [Trusting]                             INT          NULL,
    [GoodAttachment]                       INT          NULL,
    [Competitive]                          INT          NULL,
    [SelfConfidence]                       INT          NULL,
    [NoDepression]                         INT          NULL,
    [Leadership]                           INT          NULL,
    [Identity]                             INT          NULL,
    [NoSocialAnxiety]                      INT          NULL,
    [LikesParties]                         INT          NULL,
    [LikesCrowds]                          INT          NULL,
    [ExperienceSeeking]                    INT          NULL,
    [Exhibitionistic]                      INT          NULL,
    [Entertaining]                         INT          NULL,
    [EasyToLiveWith]                       INT          NULL,
    [Sensitive]                            INT          NULL,
    [Caring]                               INT          NULL,
    [LikesPeople]                          INT          NULL,
    [NoHostility]                          INT          NULL,
    [Moralistic]                           INT          NULL,
    [Mastery]                              INT          NULL,
    [Virtuous]                             INT          NULL,
    [NotAutonomous]                        INT          NULL,
    [NotSpontaneous]                       INT          NULL,
    [ImpulseControl]                       INT          NULL,
    [AvoidsTrouble]                        INT          NULL,
    [ScienceAbility]                       INT          NULL,
    [Curiosity]                            INT          NULL,
    [ThrillSeeking]                        INT          NULL,
    [IntellectualGames]                    INT          NULL,
    [GeneratesIdeas]                       INT          NULL,
    [Culture]                              INT          NULL,
    [Education]                            INT          NULL,
    [MathAbility]                          INT          NULL,
    [GoodMemory]                           INT          NULL,
    [Reading]                              INT          NULL,
    [RValidity]                            INT          NULL,
    [RAdjustment]                          INT          NULL,
    [RAmbition]                            INT          NULL,
    [RSociability]                         INT          NULL,
    [RInterpersonalSensitivity]            INT          NULL,
    [RPrudence]                            INT          NULL,
    [RInquisitive]                         INT          NULL,
    [RLearningApproach]                    INT          NULL,
    [RServiceOrientation]                  INT          NULL,
    [RStressTolerance]                     INT          NULL,
    [RReliability]                         INT          NULL,
    [RClericalPotential]                   INT          NULL,
    [RSalesPotential]                      INT          NULL,
    [RManagerialPotential]                 INT          NULL,
    [PValidity]                            INT          NULL,
    [PAdjustment]                          INT          NULL,
    [PAmbition]                            INT          NULL,
    [PSociability]                         INT          NULL,
    [PInterpersonalSensitivity]            INT          NULL,
    [PPrudence]                            INT          NULL,
    [PInquisitive]                         INT          NULL,
    [PLearningApproach]                    INT          NULL,
    [PServiceOrientation]                  INT          NULL,
    [PStressTolerance]                     INT          NULL,
    [PReliability]                         INT          NULL,
    [PClericalPotential]                   INT          NULL,
    [PSalesPotential]                      INT          NULL,
    [PManagerialPotential]                 INT          NULL,
    [HDSDate]                              VARCHAR (50) NULL,
    [RExcitable]                           INT          NULL,
    [RSkeptical]                           INT          NULL,
    [RCautious]                            INT          NULL,
    [RReserved]                            INT          NULL,
    [RLeisurely]                           INT          NULL,
    [RBold]                                INT          NULL,
    [RMischievous]                         INT          NULL,
    [RColorful]                            INT          NULL,
    [RImaginative]                         INT          NULL,
    [RDiligent]                            INT          NULL,
    [RDutiful]                             INT          NULL,
    [RUnlikeness]                          INT          NULL,
    [PExcitable]                           INT          NULL,
    [PSkeptical]                           INT          NULL,
    [PCautious]                            INT          NULL,
    [PReserved]                            INT          NULL,
    [PLeisurely]                           INT          NULL,
    [PBold]                                INT          NULL,
    [PMischievous]                         INT          NULL,
    [PColorful]                            INT          NULL,
    [PImaginative]                         INT          NULL,
    [PDiligent]                            INT          NULL,
    [PDutiful]                             INT          NULL,
    [PUnlikeness]                          INT          NULL,
    [MVPIDate]                             VARCHAR (50) NULL,
    [RAesthetic_Lifestyles]                INT          NULL,
    [RAesthetic_Beliefs]                   INT          NULL,
    [RAesthetic_Occupational_Prferences]   INT          NULL,
    [RAesthetic_Aversions]                 INT          NULL,
    [RAesthetic_Preferred_Associates]      INT          NULL,
    [RAffiliation_Lifestyles]              INT          NULL,
    [RAffiliation_Beliefs]                 INT          NULL,
    [RAffiliation_Occupational_Prferences] INT          NULL,
    [RAffiliation_Aversions]               INT          NULL,
    [RAffiliation_Preferred_Associates]    INT          NULL,
    [RAltruistic_Lifestyles]               INT          NULL,
    [RAltruistic_Beliefs]                  INT          NULL,
    [RAltruistic_Occupational_Prferences]  INT          NULL,
    [RAltruistic_Aversions]                INT          NULL,
    [RAltruistic_Preferred_Associates]     INT          NULL,
    [RCommercial_Lifestyles]               INT          NULL,
    [RCommercial_Beliefs]                  INT          NULL,
    [RCommercial_Occupational_Prferences]  INT          NULL,
    [RCommercial_Aversions]                INT          NULL,
    [RCommercial_Preferred_Associates]     INT          NULL,
    [RHedonistic_Lifestyles]               INT          NULL,
    [RHedonistic_Beliefs]                  INT          NULL,
    [RHedonistic_Occupational_Prferences]  INT          NULL,
    [RHedonistic_Aversions]                INT          NULL,
    [RHedonistic_Preferred_Associates]     INT          NULL,
    [RPower_Lifestyles]                    INT          NULL,
    [RPower_Beliefs]                       INT          NULL,
    [RPower_Occupational_Prferences]       INT          NULL,
    [RPower_Aversions]                     INT          NULL,
    [RPower_Preferred_Associates]          INT          NULL,
    [RRecognition_Lifestyles]              INT          NULL,
    [RRecognition_Beliefs]                 INT          NULL,
    [RRecognition_Occupational_Prferences] INT          NULL,
    [RRecognition_Aversions]               INT          NULL,
    [RRecognition_Preferred_Associates]    INT          NULL,
    [RScientific_Lifestyles]               INT          NULL,
    [RScientific_Beliefs]                  INT          NULL,
    [RScientific_Occupational_Prferences]  INT          NULL,
    [RScientific_Aversions]                INT          NULL,
    [RScientific_Preferred_Associates]     INT          NULL,
    [RSecurity_Lifestyles]                 INT          NULL,
    [RSecurity_Beliefs]                    INT          NULL,
    [RSecurity_Occupational_Prferences]    INT          NULL,
    [RSecurity_Aversions]                  INT          NULL,
    [RSecurity_Preferred_Associates]       INT          NULL,
    [RTradition_Lifestyles]                INT          NULL,
    [RTradition_Beliefs]                   INT          NULL,
    [RTradition_Occupational_Prferences]   INT          NULL,
    [RTradition_Aversions]                 INT          NULL,
    [RTradition_Preferred_Associates]      INT          NULL,
    [RAesthetic]                           INT          NULL,
    [RAffiliation]                         INT          NULL,
    [RAltruistic]                          INT          NULL,
    [RCommercial]                          INT          NULL,
    [RHedonistic]                          INT          NULL,
    [RPower]                               INT          NULL,
    [RRecognition]                         INT          NULL,
    [RScientific]                          INT          NULL,
    [RSecurity]                            INT          NULL,
    [RTradition]                           INT          NULL,
    [PAesthetic]                           INT          NULL,
    [PAffiliation]                         INT          NULL,
    [PAltruistic]                          INT          NULL,
    [PCommercial]                          INT          NULL,
    [PHedonistic]                          INT          NULL,
    [PPower]                               INT          NULL,
    [PRecognition]                         INT          NULL,
    [PScientific]                          INT          NULL,
    [PSecurity]                            INT          NULL,
    [PTradition]                           INT          NULL,
    [LastUpdated] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    CONSTRAINT [PK_Manual_Hogan_Import] PRIMARY KEY CLUSTERED ([Id] ASC)
);

