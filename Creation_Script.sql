USE master;

CREATE DATABASE CoTEC_DB;
GO


USE CoTEC_DB;
CREATE LOGIN CoTEC_ServerApp_Login WITH PASSWORD = '2Lfy.KMNwe{{>&@AZ&A3';
GO

CREATE USER CoTEC_ServerApp FOR LOGIN CoTEC_ServerApp_Login WITH DEFAULT_SCHEMA = admin;
GO

CREATE SCHEMA admin AUTHORIZATION CoTEC_ServerApp;
GO

CREATE SCHEMA healthcare AUTHORIZATION CoTEC_ServerApp;
GO

CREATE SCHEMA user_public AUTHORIZATION CoTEC_ServerApp;
GO

EXEC sp_addrolemember 'db_datawriter', 'CoTEC_ServerApp'
GO

EXEC sp_addrolemember 'db_datareader', 'CoTEC_ServerApp'
GO

CREATE TABLE admin.Continent(
	Name VARCHAR(100) PRIMARY KEY
);
GO

CREATE TABLE admin.country(
	Name VARCHAR(100),
	Continent VARCHAR(100),

	PRIMARY KEY(Name),
	FOREIGN KEY(Continent) REFERENCES admin.Continent(Name)
);
GO

CREATE TABLE admin.region(
	Name VARCHAR(100),
	country VARCHAR(100),

	PRIMARY KEY(Name, country),
	FOREIGN KEY(country) REFERENCES admin.country(Name)
);
GO



CREATE TABLE user_public.Population(
	day Date,
	country_Name VARCHAR(100),
	Infected int NOT NULL CHECK (Infected >= 0),
	Cured int NOT NULL CHECK (Cured >= 0),
	Dead int NOT NULL CHECK (Dead >= 0),
	Active AS Infected - Cured - Dead PERSISTED NOT NULL CHECK (Active >= 0),

	PRIMARY KEY(day, country_Name),
	FOREIGN KEY(country_Name) REFERENCES admin.country(Name)
);
GO

CREATE TABLE admin.Contention_Measure(
	Name VARCHAR(80),
	Description VARCHAR(5000) DEFAULT 'No Description',

	PRIMARY KEY(Name)
);
GO

CREATE TABLE user_public.Contention_Measures_Changes(
	country_Name VARCHAR(100),
	Measure_Name VARCHAR(80),
	Start_Date Date,
	End_Date Date,

	PRIMARY KEY(Measure_Name, country_Name, Start_Date),
	FOREIGN KEY(country_Name) REFERENCES admin.country(Name),
	FOREIGN KEY(Measure_Name) REFERENCES admin.Contention_Measure(Name)
);
GO

CREATE TABLE admin.Sanitary_Measure(
	Name VARCHAR(100),
	Description VARCHAR(5000) DEFAULT 'No Description',

	PRIMARY KEY(Name)
);
GO

CREATE TABLE user_public.Sanitary_Measures_Changes(
	country_Name VARCHAR(100),
	Measure_Name VARCHAR(100),
	Start_Date Date,
	End_Date Date,

	PRIMARY KEY(Measure_Name, country_Name, Start_Date),
	FOREIGN KEY(country_Name) REFERENCES admin.country(Name),
	FOREIGN KEY(Measure_Name) REFERENCES admin.Sanitary_Measure(Name)
);
GO

CREATE TABLE admin.Medication(
	Medicine VARCHAR(80),
	Pharmacist VARCHAR(80) DEFAULT 'Unknown',
	PRIMARY KEY(Medicine)
);
GO

CREATE TABLE admin.Patient_State(
	name VARCHAR(50),
	PRIMARY KEY(Name)
);
GO

CREATE TABLE admin.Staff(
	Id_Code NVARCHAR(50) PRIMARY KEY,
	firstName VARCHAR(1000) NOT NULL,
	lastName VARCHAR(1000) NOT NULL,
	Password NVARCHAR(2000) NOT NULL,
	Role VARCHAR(20) NOT NULL
);
GO

CREATE TABLE admin.Pathology(
	Name VARCHAR(150) PRIMARY KEY,
	Description VARCHAR(5000) DEFAULT 'No Description',
	Treatment VARCHAR(5000) DEFAULT 'No Treatment',
);
GO

CREATE TABLE healthcare.Contact(
	identification NVARCHAR(50),
	firstName VARCHAR(1000) NOT NULL,
	lastName VARCHAR(1000) NOT NULL,
	region VARCHAR(100) NOT NULL,
	nationality VARCHAR(100) DEFAULT 'No nationality',
	country VARCHAR(100) NOT NULL,
	address VARCHAR(500) NOT NULL,
	email VARCHAR(200) NOT NULL,
	age tinyint DEFAULT 0 CHECK (age >= 0),

	PRIMARY KEY(identification),
	FOREIGN KEY(region, country) REFERENCES admin.region(Name, country)
);
GO

CREATE TABLE admin.Hospital(
	Name VARCHAR(80),
	region VARCHAR(100),
	country VARCHAR(100),
	Capacity smallint NOT NULL CHECK (Capacity >= 0),
	ICU_Capacity smallint NOT NULL CHECK (ICU_Capacity >= 0),
	Manager NVARCHAR(50) NOT NULL,

	PRIMARY KEY(Name, region, country),
	FOREIGN KEY(region, country) REFERENCES admin.region(Name, country),
	FOREIGN KEY(Manager) REFERENCES healthcare.Contact(identification)
);
GO

CREATE TABLE healthcare.Patient(
	identification NVARCHAR(50),
	firstName VARCHAR(1000) NOT NULL,
	lastName VARCHAR(1000) NOT NULL,
	region VARCHAR(100) NOT NULL,
	nationality VARCHAR(100) NOT NULL,
	country VARCHAR(100) NOT NULL,
	age tinyint NOT NULL CHECK (age >= 0),
	intensiveCareUnite bit NOT NULL DEFAULT 0,
	hospitalized bit NOT NULL  DEFAULT 0,
	state VARCHAR(50) NOT NULL,
	Date_Entrance Date NOT NULL,

	PRIMARY KEY(identification),
	FOREIGN KEY(region, country) REFERENCES admin.region(Name, country),
	FOREIGN KEY(state) REFERENCES admin.Patient_State(Name)
);
GO

CREATE TABLE admin.Hospital_Workers(
	Id_Code NVARCHAR(50),
	Name VARCHAR(80),
	region VARCHAR(100),
	country VARCHAR(100),

	PRIMARY KEY(Id_Code, Name, region, country),
	FOREIGN KEY(Name, region, country) REFERENCES admin.Hospital(Name, region, country),
	FOREIGN KEY(Id_Code) REFERENCES admin.Staff(Id_Code)
);
GO

CREATE TABLE healthcare.Patient_Medication(
	Patient_Id NVARCHAR(50),
	Medication VARCHAR(80),

	PRIMARY KEY(Patient_Id, Medication),
	FOREIGN KEY(Patient_Id) REFERENCES healthcare.Patient(identification),
	FOREIGN KEY(Medication) REFERENCES admin.Medication(Medicine)
);
GO

CREATE TABLE healthcare.Patient_Pathology(
	Pathology_Name VARCHAR(150),
	Patient_Id NVARCHAR(50),

	PRIMARY KEY(Pathology_Name, Patient_Id),
	FOREIGN KEY(Patient_Id) REFERENCES healthcare.Patient(identification),
	FOREIGN KEY(Pathology_Name) REFERENCES admin.Pathology(Name)
);
GO

CREATE TABLE admin.Pathology_Symptoms(
	Pathology VARCHAR(150),
	Symptom VARCHAR(100),

	PRIMARY KEY(Pathology, Symptom),
	FOREIGN KEY(Pathology) REFERENCES admin.Pathology(Name)
);
GO

CREATE TABLE healthcare.Contact_Pathology(
	Pathology_name VARCHAR(150),
	Contact_Id NVARCHAR(50),

	PRIMARY KEY(Contact_Id, Pathology_name),
	FOREIGN KEY(Contact_Id) REFERENCES healthcare.Contact(identification),
	FOREIGN KEY(Pathology_name) REFERENCES admin.Pathology(Name)
);
GO

CREATE TABLE healthcare.Patient_Contact(
	Patient_Id NVARCHAR(50),
	Contact_Id NVARCHAR(50),
	last_visit Date NOT NULL DEFAULT CONVERT(Date, GETDATE()),

	PRIMARY KEY(Contact_Id, Patient_Id, last_visit),
	FOREIGN KEY(Contact_Id) REFERENCES healthcare.Contact(identification),
	FOREIGN KEY(Patient_Id) REFERENCES healthcare.Patient(identification)
);
GO

CREATE TRIGGER admin.Patient_States_Deletion_Checker
ON admin.Patient_State
FOR UPDATE, DELETE
AS
BEGIN
DECLARE @Default_States TABLE (Name varchar(50))
insert into @Default_States values('Active'),('Infected'),('Recovered'),('Deceased')

DECLARE @TEXT VARCHAR(40) = (SELECT Name FROM deleted);

IF EXISTS(SELECT Name 
			FROM deleted as D
			WHERE D.Name IN (SELECT Name FROM @Default_States))
BEGIN
	ROLLBACK TRANSACTION;
END

END
GO

CREATE TRIGGER healthcare.Cases_Count_Incrementer
ON healthcare.Patient
AFTER INSERT, DELETE
AS
BEGIN

DECLARE @New_Patients int = (SELECT COUNT(*) FROM inserted);
DECLARE @Erased_Patients int = (SELECT COUNT(*) FROM deleted);

DECLARE @state VARCHAR(80);
DECLARE @country VARCHAR(100);
DECLARE @Date_Existence INT;

IF (@New_Patients > 0)
BEGIN

	SET @state = (SELECT state FROM inserted);

	SET @country = (SELECT country FROM inserted);

	SET @Date_Existence = (SELECT COUNT(*)
				FROM user_public.Population AS p, inserted
				WHERE p.day = inserted.Date_Entrance
				AND p.country_Name = @country);


	IF (@Date_Existence > 0)

		BEGIN

			UPDATE user_public.Population
			SET Infected = Infected + 1,
				Cured = (CASE @state
							WHEN 'Recovered' THEN Cured + 1
							ELSE Cured
							END),
				Dead = (CASE @state
							WHEN 'Deceased' THEN Dead + 1
							ELSE Dead
							END)
			WHERE EXISTS(SELECT p.day
				FROM user_public.Population AS p, inserted
				WHERE p.day = inserted.Date_Entrance
				AND p.country_Name = @country)
		END

	ELSE
		BEGIN
			
			INSERT INTO user_public.Population(Infected, Cured, Dead, day, country_Name)
			VALUES(
				1,
				(CASE @state
							WHEN 'Recovered' THEN 1
							ELSE 0 END),
				(CASE @state
							WHEN 'Deceased' THEN 1
							ELSE 0 END),
				(SELECT Date_Entrance FROM inserted),
				@country)
		END
END

IF (@Erased_Patients > 0)
BEGIN

	SET @state = (SELECT state FROM deleted);

	SET @country = (SELECT country FROM deleted);

	SET @Date_Existence = (SELECT COUNT(*)
				FROM user_public.Population AS p, deleted
				WHERE p.day = deleted.Date_Entrance
				AND p.country_Name = @country);

	IF (@Date_Existence > 0)
		BEGIN
			UPDATE user_public.Population
			SET Infected = Infected - 1,
				Cured = (CASE @state
							WHEN 'Recovered' THEN Cured - 1
							ELSE Cured
							END),
				Dead = (CASE @state
							WHEN 'Deceased' THEN Dead - 1
							ELSE Dead
							END)
			WHERE EXISTS(SELECT p.day
				FROM user_public.Population AS p, deleted
				WHERE p.day = deleted.Date_Entrance
				AND p.country_Name = @country)

			DELETE
			FROM user_public.Population
			WHERE day = (SELECT Date_Entrance FROM deleted)
			AND country_Name = @country
			AND Infected = 0
			AND Cured = 0
			AND Dead = 0
		END
END
END
GO

CREATE PROCEDURE healthcare.Patients_updater (@identification NVARCHAR(50),
													@firstName VARCHAR(1000) = NULL,
													@lastName VARCHAR(1000) = NULL,
													@region  VARCHAR(100) = NULL,
													@nationality VARCHAR(100) = NULL,
													@country VARCHAR(100) = NULL,
													@age TINYINT = NULL,
													@intensiveCareUnite BIT = NULL,
													@hospitalized BIT = NULL,
													@state VARCHAR(50) = NULL,
													@Entrance_Date Date = NULL)
AS

BEGIN

IF @identification IS NULL RAISERROR (15600,-1,-1, 'mysp_Cases_Count_updater'); 

DECLARE @Old_Date Date;
DECLARE @Old_State VARCHAR(50);
DECLARE @Old_country VARCHAR(50);


SELECT @Old_country = country, @Old_Date = Date_Entrance, @Old_State = state
FROM healthcare.Patient
WHERE identification = @identification;

BEGIN TRY

	UPDATE healthcare.Patient
SET firstName =	(CASE
						WHEN @firstName is null THEN firstName
						ELSE @firstName END),
	lastName =		(CASE
						WHEN @lastName is null THEN lastName
						ELSE @lastName END),
	region =		(CASE
						WHEN @region is null THEN region
						ELSE @region END),
	nationality =	(CASE
						WHEN @nationality is null THEN nationality
						ELSE @nationality END),
	country =		(CASE
						WHEN @country is null THEN country
						ELSE @country END),
	intensiveCareUnite =			(CASE
						WHEN @intensiveCareUnite is null THEN intensiveCareUnite
						ELSE @intensiveCareUnite END),
	age =			(CASE
						WHEN @age is null THEN age
						ELSE @age END),
	hospitalized =	(CASE
						WHEN @hospitalized is null THEN hospitalized
						ELSE @hospitalized END),
	state =			(CASE
						WHEN @state is null THEN state
						ELSE @state END),
	Date_Entrance =	(CASE 
						WHEN @Entrance_Date is null THEN Date_Entrance
						ELSE @Entrance_Date END)
	WHERE identification = @identification;

END TRY

BEGIN CATCH
	SELECT  
            ERROR_NUMBER() AS ErrorNumber  
            ,ERROR_SEVERITY() AS ErrorSeverity  
            ,ERROR_STATE() AS ErrorState  
            ,ERROR_PROCEDURE() AS ErrorProcedure  
            ,ERROR_LINE() AS ErrorLine  
            ,ERROR_MESSage() AS ErrorMessage;
	RETURN;
END CATCH


IF (@country IS NULL) SET @country = @Old_country;
IF (@Entrance_Date IS NULL) SET @Entrance_Date = @Old_Date;
IF (@state IS NULL) SET @state = @Old_State;



IF (@Old_country = @country AND
	@Old_Date = @Entrance_Date AND
	@Old_State != @state)
	
	BEGIN

		UPDATE user_public.Population
		SET	Cured = (CASE @Old_State
						WHEN 'Recovered' THEN Cured - 1
						ELSE Cured END),
			Dead = (CASE @Old_State
						WHEN 'Deceased' THEN Dead - 1
						ELSE Dead END)
		WHERE day = @Old_Date
		AND country_Name = @Old_country;

		UPDATE user_public.Population
		SET	Cured = (CASE @state
						WHEN 'Recovered' THEN Cured + 1
						ELSE Cured END),
			Dead = (CASE @state
						WHEN 'Deceased' THEN Dead + 1
						ELSE Dead END)
		WHERE day = @Entrance_Date
		AND country_Name = @country;


	END;
	
ELSE IF (@Old_country != @country OR
	@Old_Date != @Entrance_Date OR
	@Old_State != @state)

	BEGIN
	
		UPDATE user_public.Population
		SET Infected = Infected - 1,
			Cured = (CASE @Old_State
						WHEN 'Recovered' THEN Cured - 1
						ELSE Cured END),
			Dead = (CASE @Old_State
						WHEN 'Deceased' THEN Dead - 1
						ELSE Dead END)
		WHERE day = @Old_Date
		AND country_Name = @Old_country;

		IF EXISTS(SELECT day, country_Name
					FROM user_public.Population
					WHERE country_Name = @country
					AND day = @Entrance_Date)
			BEGIN
		
				UPDATE user_public.Population
				SET Infected = Infected + 1,
					Cured = (CASE @state
								WHEN 'Recovered' THEN Cured + 1
								ELSE Cured END),
					Dead = (CASE @state
								WHEN 'Deceased' THEN Dead + 1
								ELSE Dead END)
				WHERE day = @Entrance_Date
				AND country_Name = @country;
			END;

		ELSE

			BEGIN
			
				INSERT INTO user_public.Population(country_Name, day, Infected, Cured, Dead)
				VALUES(
					@country,
					@Entrance_Date,
					1,
					(CASE @state
								WHEN 'Recovered' THEN 1
								ELSE 0 END),
					(CASE @state
								WHEN 'Deceased' THEN 1
								ELSE 0 END));
			END;
	END;

END;
GO

CREATE PROCEDURE healthcare.Patient_Existence_Checker (@identification NVARCHAR(50),
													@firstName VARCHAR(1000),
													@lastName VARCHAR(1000),
													@region  VARCHAR(100),
													@nationality VARCHAR(100) = 'No nationality',
													@country VARCHAR(100),
													@age TINYINT,
													@intensiveCareUnite BIT = 0,
													@hospitalized BIT = 0,
													@state VARCHAR(50),
													@Entrance_Date Date)
AS
BEGIN

IF @identification IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @firstName IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @lastName IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @region IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Entrance_Date IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @country IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @age IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @state IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');

	IF NOT EXISTS(SELECT identification
				FROM healthcare.Patient
				WHERE @identification = identification)

		BEGIN
			
			INSERT INTO healthcare.Patient
			VALUES(
				@identification,
				@firstName,
				@lastName,
				@region,
				@nationality,
				@country,
				@age,
				@intensiveCareUnite,
				@hospitalized,
				@state,
				@Entrance_Date);
		END
END;
GO

CREATE PROCEDURE healthcare.Contact_Existence_Checker (@identification NVARCHAR(50),
													@firstName VARCHAR(1000),
													@lastName VARCHAR(1000),
													@region  VARCHAR(100),
													@nationality VARCHAR(100) = 'No nationality',
													@country VARCHAR(100),
													@Address VARCHAR(500),
													@email VARCHAR(200),
													@age TINYINT)
AS
BEGIN

IF @identification IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @firstName IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @lastName IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @region IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Address IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @country IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @age IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @email IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');

	IF NOT EXISTS(SELECT identification
				FROM healthcare.Contact
				WHERE @identification = identification)

		BEGIN
			
			INSERT INTO healthcare.Contact
			VALUES(
				@identification,
				@firstName,
				@lastName,
				@region,
				@nationality,
				@country,
				@Address,
				@email,
				@age);
		END
END;
GO

INSERT INTO admin.Patient_State VALUES ('Active'),('Infected'),('Recovered'),('Deceased');
GO

