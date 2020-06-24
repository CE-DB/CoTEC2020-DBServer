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
	Name VARCHAR(100) PRIMARY KEY,
	Continent VARCHAR(100) NOT NULL,

	FOREIGN KEY(Continent) REFERENCES admin.Continent(Name)
);
GO

CREATE TABLE admin.Region(
	Name VARCHAR(100),
	Country VARCHAR(100),

	PRIMARY KEY(name, Country),
	FOREIGN KEY(Country) REFERENCES admin.Country(Name)
);
GO

CREATE TABLE user_public.Population(
	Date Date,
	Country_Name VARCHAR(100),
	Infected int NOT NULL,
	Cured int NOT NULL,
	Dead int NOT NULL,
	Active AS Infected - Cured - Dead PERSISTED NOT NULL,

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
	Date_Entrance AS CONVERT(date, GETDATE())

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

