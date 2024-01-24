CREATE DATABASE UserDB;
GO
USE UserDB;
GO
CREATE TABLE Users 
    (id UNIQUEIDENTIFIER NOT NULL,
     created datetime DEFAULT CURRENT_TIMESTAMP,
     updated datetime,
     user_name NVARCHAR(255) NOT NULL,
     pass_hash NVARCHAR(MAX) NOT NULL,
     first_name NVARCHAR(255) NOT NULL,
     family_name NVARCHAR(255) NOT NULL,
     email NVARCHAR(255) NOT NULL,
     email_verified BIT NOT NULL DEFAULT 0,
     CONSTRAINT [PK_Users] PRIMARY KEY NonCLUSTERED ([id] ASC));
GO
INSERT INTO UserDB.dbo.Users (id,created,updated,user_name,pass_hash,first_name,family_name,email,email_verified)
VALUES ('93ccce56-45a6-4694-be92-187df5dfe0a8',CURRENT_TIMESTAMP,NULL,'RootUser','L1/xfccGJ2sempE1TY5jtVL91Aa+RXQcf2Bs2nPFZpgTLuMPFwWoA5vI/vawd6xlHhnRuUbIyzZlwgDzSQAKqRzW4sTguL9TKsrWAj26e5/ip1e4xp67XW5ZWTHJk/ZXDE/7KsaDz0M4oL+MEwHVInVVP02so0VoShnSpqrAADORHKkgc1QHEAbgz/g2OxINEDWVAZc9rqqBRGXkI1O0Ha4+VzN2WDMIqkZKwngu3URpp5ROaZ8Duvrq4VNe9RK8VsGJPg2GZMGc7bhgEP8m++qBBRWImspHJqKc4zRBawT/wemPedUn1RgJQpTZbXuP+35Tfr7h222dZrWj7FN1hQ==','Root','User','rootuser@circle.com',0)
GO
CREATE TABLE Tokens
    (user_id UNIQUEIDENTIFIER NOT NULL,
     at_valide_from datetime,
     at_valide_to datetime,
     at_value NVARCHAR(MAX),
     rt_valide_from datetime,
     rt_valide_to datetime,
     rt_value NVARCHAR(MAX),
     CONSTRAINT [PK_Tokens] PRIMARY KEY NonCLUSTERED ([user_id] ASC));
GO