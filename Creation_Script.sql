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

CREATE TABLE admin.Country(
	Name VARCHAR(100),
	Continent VARCHAR(100),

	PRIMARY KEY(Name),
	FOREIGN KEY(Continent) REFERENCES admin.Continent(Name)
);
GO

CREATE TABLE admin.Region(
	Name VARCHAR(100),
	Country VARCHAR(100),

	PRIMARY KEY(Name, Country),
	FOREIGN KEY(Country) REFERENCES admin.Country(Name)
);
GO



CREATE TABLE user_public.Population(
	Date Date,
	Country_Name VARCHAR(100),
	Infected int NOT NULL CHECK (Infected >= 0),
	Cured int NOT NULL CHECK (Cured >= 0),
	Dead int NOT NULL CHECK (Dead >= 0),
	Active AS Infected - Cured - Dead PERSISTED NOT NULL CHECK (Active >= 0),

	PRIMARY KEY(Date, Country_Name),
	FOREIGN KEY(Country_Name) REFERENCES admin.Country(Name)
);
GO

CREATE TABLE admin.Contention_Measure(
	Name VARCHAR(80),
	Description VARCHAR(5000) NOT NULL,

	PRIMARY KEY(Name)
);
GO

CREATE TABLE user_public.Contention_Measures_Changes(
	Country_Name VARCHAR(100),
	Measure_Name VARCHAR(80),
	Start_Date Date,
	End_Date Date NOT NULL,

	PRIMARY KEY(Measure_Name, Country_Name, Start_Date),
	FOREIGN KEY(Country_Name) REFERENCES admin.Country(Name),
	FOREIGN KEY(Measure_Name) REFERENCES admin.Contention_Measure(Name)
);
GO

CREATE TABLE admin.Sanitary_Measure(
	Name VARCHAR(100),
	Description VARCHAR(5000) NOT NULL,

	PRIMARY KEY(Name)
);
GO

CREATE TABLE user_public.Sanitary_Measures_Changes(
	Country_Name VARCHAR(100),
	Measure_Name VARCHAR(100),
	Start_Date Date,
	End_Date Date NOT NULL,

	PRIMARY KEY(Measure_Name, Country_Name, Start_Date),
	FOREIGN KEY(Country_Name) REFERENCES admin.Country(Name),
	FOREIGN KEY(Measure_Name) REFERENCES admin.Sanitary_Measure(Name)
);
GO

CREATE TABLE admin.Medication(
	Medicine VARCHAR(80),
	Pharmacist VARCHAR(80) NOT NULL,
	PRIMARY KEY(Medicine)
);
GO

CREATE TABLE admin.Patient_State(
	Name VARCHAR(50),
	PRIMARY KEY(Name)
);
GO

CREATE TABLE admin.Staff(
	Id_Code NVARCHAR(50) PRIMARY KEY,
	First_Name VARCHAR(1000) NOT NULL,
	Last_Name VARCHAR(1000) NOT NULL,
	Password NVARCHAR(2000) NOT NULL,
	Role VARCHAR(20) NOT NULL
);
GO

CREATE TABLE admin.Pathology(
	Name VARCHAR(150) PRIMARY KEY,
	Description VARCHAR(5000) NOT NULL DEFAULT 'No Description',
	Treatment VARCHAR(5000) NOT NULL DEFAULT 'No Treatment',
);
GO

CREATE TABLE healthcare.Contact(
	Id_Number NVARCHAR(50),
	First_Name VARCHAR(1000) NOT NULL,
	Last_Name VARCHAR(1000) NOT NULL,
	Region VARCHAR(100) NOT NULL,
	Nationality VARCHAR(100) NOT NULL DEFAULT 'No nationality',
	Country VARCHAR(100) NOT NULL,
	Address VARCHAR(500) NOT NULL,
	Email VARCHAR(200) NOT NULL,
	Age tinyint NOT NULL CHECK (Age >= 0)

	PRIMARY KEY(Id_Number),
	FOREIGN KEY(Region, Country) REFERENCES admin.Region(Name, Country)
);
GO

CREATE TABLE admin.Hospital(
	Name VARCHAR(80),
	Region VARCHAR(100),
	Country VARCHAR(100),
	Capacity smallint NOT NULL CHECK (Capacity >= 0),
	ICU_Capacity smallint NOT NULL CHECK (ICU_Capacity >= 0),
	Manager NVARCHAR(50) NOT NULL,

	PRIMARY KEY(Name, Region, Country),
	FOREIGN KEY(Region, Country) REFERENCES admin.Region(Name, Country),
	FOREIGN KEY(Manager) REFERENCES healthcare.Contact(Id_Number)
);
GO

CREATE TABLE healthcare.Patient(
	Id_Number NVARCHAR(50),
	First_Name VARCHAR(1000) NOT NULL,
	Last_Name VARCHAR(1000) NOT NULL,
	Region VARCHAR(100) NOT NULL,
	Nationality VARCHAR(100) NOT NULL,
	Country VARCHAR(100) NOT NULL,
	Age tinyint NOT NULL CHECK (Age >= 0),
	ICU bit NOT NULL DEFAULT 0,
	Hospitalized bit NOT NULL  DEFAULT 0,
	State VARCHAR(50) NOT NULL,
	Date_Entrance Date NOT NULL,

	PRIMARY KEY(Id_Number),
	FOREIGN KEY(Region, Country) REFERENCES admin.Region(Name, Country),
	FOREIGN KEY(State) REFERENCES admin.Patient_State(Name)
);
GO

CREATE TABLE admin.Hospital_Workers(
	Id_Code NVARCHAR(50),
	Name VARCHAR(80),
	Region VARCHAR(100),
	Country VARCHAR(100),

	PRIMARY KEY(Id_Code, Name, Region, Country),
	FOREIGN KEY(Name, Region, Country) REFERENCES admin.Hospital(Name, Region, Country),
	FOREIGN KEY(Id_Code) REFERENCES admin.Staff(Id_Code)
);
GO

CREATE TABLE healthcare.Patient_Medication(
	Patient_Id NVARCHAR(50),
	Medication VARCHAR(80),

	PRIMARY KEY(Patient_Id, Medication),
	FOREIGN KEY(Patient_Id) REFERENCES healthcare.Patient(Id_Number),
	FOREIGN KEY(Medication) REFERENCES admin.Medication(Medicine)
);
GO

CREATE TABLE healthcare.Patient_Pathology(
	Pathology_Name VARCHAR(150),
	Patient_Id NVARCHAR(50),

	PRIMARY KEY(Pathology_Name, Patient_Id),
	FOREIGN KEY(Patient_Id) REFERENCES healthcare.Patient(Id_Number),
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
	FOREIGN KEY(Contact_Id) REFERENCES healthcare.Contact(Id_Number),
	FOREIGN KEY(Pathology_name) REFERENCES admin.Pathology(Name)
);
GO

CREATE TABLE healthcare.Patient_Contact(
	Patient_Id NVARCHAR(50),
	Contact_Id NVARCHAR(50),

	PRIMARY KEY(Contact_Id, Patient_Id),
	FOREIGN KEY(Contact_Id) REFERENCES healthcare.Contact(Id_Number),
	FOREIGN KEY(Patient_Id) REFERENCES healthcare.Patient(Id_Number)
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

DECLARE @State VARCHAR(80);
DECLARE @Country VARCHAR(100);
DECLARE @Date_Existence INT;

IF (@New_Patients > 0)
BEGIN

	SET @State = (SELECT State FROM inserted);

	SET @Country = (SELECT Country FROM inserted);

	SET @Date_Existence = (SELECT COUNT(*)
				FROM user_public.Population AS p, inserted
				WHERE p.Date = inserted.Date_Entrance
				AND p.Country_Name = @Country);


	IF (@Date_Existence > 0)

		BEGIN

			UPDATE user_public.Population
			SET Infected = Infected + 1,
				Cured = (CASE @State
							WHEN 'Recovered' THEN Cured + 1
							ELSE Cured
							END),
				Dead = (CASE @State
							WHEN 'Deceased' THEN Dead + 1
							ELSE Dead
							END)
			WHERE EXISTS(SELECT p.Date
				FROM user_public.Population AS p, inserted
				WHERE p.Date = inserted.Date_Entrance
				AND p.Country_Name = @Country)
		END

	ELSE
		BEGIN
			
			INSERT INTO user_public.Population(Infected, Cured, Dead, Date, Country_Name)
			VALUES(
				1,
				(CASE @State
							WHEN 'Recovered' THEN 1
							ELSE 0 END),
				(CASE @State
							WHEN 'Deceased' THEN 1
							ELSE 0 END),
				(SELECT Date_Entrance FROM inserted),
				@Country)
		END
END

IF (@Erased_Patients > 0)
BEGIN

	SET @State = (SELECT State FROM deleted);

	SET @Country = (SELECT Country FROM deleted);

	SET @Date_Existence = (SELECT COUNT(*)
				FROM user_public.Population AS p, deleted
				WHERE p.Date = deleted.Date_Entrance
				AND p.Country_Name = @Country);

	IF (@Date_Existence > 0)
		BEGIN
			UPDATE user_public.Population
			SET Infected = Infected - 1,
				Cured = (CASE @State
							WHEN 'Recovered' THEN Cured - 1
							ELSE Cured
							END),
				Dead = (CASE @State
							WHEN 'Deceased' THEN Dead - 1
							ELSE Dead
							END)
			WHERE EXISTS(SELECT p.Date
				FROM user_public.Population AS p, deleted
				WHERE p.Date = deleted.Date_Entrance
				AND p.Country_Name = @Country)
		END
END
END
GO

CREATE PROCEDURE healthcare.Patients_updater (@id_number NVARCHAR(50),
													@First_Name VARCHAR(1000) = NULL,
													@Last_Name VARCHAR(1000) = NULL,
													@Region  VARCHAR(100) = NULL,
													@Nationality VARCHAR(100) = NULL,
													@Country VARCHAR(100) = NULL,
													@Age TINYINT = NULL,
													@ICU BIT = NULL,
													@Hospitalized BIT = NULL,
													@State VARCHAR(50) = NULL,
													@Entrance_Date Date = NULL)
AS

BEGIN

IF @id_number IS NULL RAISERROR (15600,-1,-1, 'mysp_Cases_Count_updater'); 

DECLARE @Old_Date Date;
DECLARE @Old_State VARCHAR(50);
DECLARE @Old_Country VARCHAR(50);


SELECT @Old_Country = Country, @Old_Date = Date_Entrance, @Old_State = State
FROM healthcare.Patient
WHERE Id_Number = @id_number;

BEGIN TRY

	UPDATE healthcare.Patient
SET First_Name =	(CASE
						WHEN @First_Name is null THEN First_Name
						ELSE @First_Name END),
	Last_Name =		(CASE
						WHEN @Last_Name is null THEN Last_Name
						ELSE @Last_Name END),
	Region =		(CASE
						WHEN @Region is null THEN Region
						ELSE @Region END),
	Nationality =	(CASE
						WHEN @Nationality is null THEN Nationality
						ELSE @Nationality END),
	Country =		(CASE
						WHEN @Country is null THEN Country
						ELSE @Country END),
	ICU =			(CASE
						WHEN @ICU is null THEN ICU
						ELSE @ICU END),
	Age =			(CASE
						WHEN @Age is null THEN Age
						ELSE @Age END),
	Hospitalized =	(CASE
						WHEN @Hospitalized is null THEN Hospitalized
						ELSE @Hospitalized END),
	State =			(CASE
						WHEN @State is null THEN State
						ELSE @State END),
	Date_Entrance =	(CASE 
						WHEN @Entrance_Date is null THEN Date_Entrance
						ELSE @Entrance_Date END)
	WHERE Id_Number = @id_number;

END TRY

BEGIN CATCH
	SELECT  
            ERROR_NUMBER() AS ErrorNumber  
            ,ERROR_SEVERITY() AS ErrorSeverity  
            ,ERROR_STATE() AS ErrorState  
            ,ERROR_PROCEDURE() AS ErrorProcedure  
            ,ERROR_LINE() AS ErrorLine  
            ,ERROR_MESSAGE() AS ErrorMessage;
	RETURN;
END CATCH


IF (@Country IS NULL) SET @Country = @Old_Country;
IF (@Entrance_Date IS NULL) SET @Entrance_Date = @Old_Date;
IF (@State IS NULL) SET @State = @Old_State;



IF (@Old_Country = @Country AND
	@Old_Date = @Entrance_Date AND
	@Old_State != @State)
	
	BEGIN

		UPDATE user_public.Population
		SET	Cured = (CASE @Old_State
						WHEN 'Recovered' THEN Cured - 1
						ELSE Cured END),
			Dead = (CASE @Old_State
						WHEN 'Deceased' THEN Dead - 1
						ELSE Dead END)
		WHERE Date = @Old_Date
		AND Country_Name = @Old_Country;

		UPDATE user_public.Population
		SET	Cured = (CASE @State
						WHEN 'Recovered' THEN Cured + 1
						ELSE Cured END),
			Dead = (CASE @State
						WHEN 'Deceased' THEN Dead + 1
						ELSE Dead END)
		WHERE Date = @Entrance_Date
		AND Country_Name = @Country;


	END;
	
ELSE IF (@Old_Country != @Country OR
	@Old_Date != @Entrance_Date OR
	@Old_State != @State)

	BEGIN
	
		UPDATE user_public.Population
		SET Infected = Infected - 1,
			Cured = (CASE @Old_State
						WHEN 'Recovered' THEN Cured - 1
						ELSE Cured END),
			Dead = (CASE @Old_State
						WHEN 'Deceased' THEN Dead - 1
						ELSE Dead END)
		WHERE Date = @Old_Date
		AND Country_Name = @Old_Country;

		IF EXISTS(SELECT Date, Country_Name
					FROM user_public.Population
					WHERE Country_Name = @Country
					AND Date = @Entrance_Date)
			BEGIN
		
				UPDATE user_public.Population
				SET Infected = Infected + 1,
					Cured = (CASE @State
								WHEN 'Recovered' THEN Cured + 1
								ELSE Cured END),
					Dead = (CASE @State
								WHEN 'Deceased' THEN Dead + 1
								ELSE Dead END)
				WHERE Date = @Entrance_Date
				AND Country_Name = @Country;
			END;

		ELSE

			BEGIN
			
				INSERT INTO user_public.Population(Country_Name, Date, Infected, Cured, Dead)
				VALUES(
					@Country,
					@Entrance_Date,
					1,
					(CASE @State
								WHEN 'Recovered' THEN 1
								ELSE 0 END),
					(CASE @State
								WHEN 'Deceased' THEN 1
								ELSE 0 END));
			END;
	END;

END;
GO

CREATE PROCEDURE healthcare.Patient_Existence_Checker (@id_number NVARCHAR(50),
													@First_Name VARCHAR(1000),
													@Last_Name VARCHAR(1000),
													@Region  VARCHAR(100),
													@Nationality VARCHAR(100) = 'No Nationality',
													@Country VARCHAR(100),
													@Age TINYINT,
													@ICU BIT = 0,
													@Hospitalized BIT = 0,
													@State VARCHAR(50),
													@Entrance_Date Date)
AS
BEGIN

IF @id_number IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @First_Name IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Last_Name IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Region IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Entrance_Date IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Country IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Age IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @State IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');

	IF NOT EXISTS(SELECT id_number
				FROM healthcare.Patient
				WHERE @id_number = Id_Number)

		BEGIN
			
			INSERT INTO healthcare.Patient
			VALUES(
				@id_number,
				@First_Name,
				@Last_Name,
				@Region,
				@Nationality,
				@Country,
				@Age,
				@ICU,
				@Hospitalized,
				@State,
				@Entrance_Date);
		END
END;
GO

CREATE PROCEDURE healthcare.Contact_Existence_Checker (@id_number NVARCHAR(50),
													@First_Name VARCHAR(1000),
													@Last_Name VARCHAR(1000),
													@Region  VARCHAR(100),
													@Nationality VARCHAR(100) = 'No Nationality',
													@Country VARCHAR(100),
													@Address VARCHAR(500),
													@Email VARCHAR(200),
													@Age TINYINT)
AS
BEGIN

IF @id_number IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @First_Name IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Last_Name IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Region IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Address IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Country IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Age IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');
IF @Email IS NULL RAISERROR (15600,-1,-1, 'mysp_Patient_Existence_Checker');

	IF NOT EXISTS(SELECT id_number
				FROM healthcare.Contact
				WHERE @id_number = Id_Number)

		BEGIN
			
			INSERT INTO healthcare.Contact
			VALUES(
				@id_number,
				@First_Name,
				@Last_Name,
				@Region,
				@Nationality,
				@Country,
				@Address,
				@Email,
				@Age);
		END
END;
GO

INSERT INTO admin.Patient_State VALUES ('Active'),('Infected'),('Recovered'),('Deceased');
GO