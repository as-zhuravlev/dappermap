------------------------ UTILS FUNCTIONS ----------------------------------------------------------------- 

CREATE OR REPLACE FUNCTION copy_parent_constraits(parent text, child text) RETURNS void LANGUAGE plpgsql AS $$
DECLARE
    parent_constraint RECORD;
    constraint_name text;
    existed_constraint RECORD;
    col character varying;
	ref_table text;
	ref_col text;
    command text;
BEGIN
  FOR parent_constraint IN SELECT con.conname, con.contype, con.conkey,	con.confrelid, con.confkey FROM pg_constraint AS con INNER JOIN pg_class AS cl ON con.conrelid = cl.relfilenode
  WHERE cl.relname = lower(parent) 
  LOOP 
    IF array_length(parent_constraint.conkey, 1) > 1 THEN
      RAISE EXCEPTION 'MULTICOLUMN CONSTRAITS ARE NOT SUPPORTED %', parent_constraint.conname;
    END IF;
    constraint_name := replace(parent_constraint.conname, lower(parent), lower(child));
    existed_constraint := row(null);
    SELECT 1 INTO existed_constraint FROM pg_constraint AS con WHERE con.conname = constraint_name;
    IF existed_constraint IS NULL THEN
	  SELECT column_name INTO col FROM information_schema.columns WHERE table_name = lower(parent) AND ordinal_position = parent_constraint.conkey[1];
      IF parent_constraint.contype = 'p' THEN --primary key
        command := 'AlTER TABLE ' || lower(child) || ' ADD CONSTRAINT ' || constraint_name || ' PRIMARY KEY (' || col || ')';
        EXECUTE command; 
      ELSIF parent_constraint.contype = 'u' THEN -- unique
        command := 'ALTER TABLE '  || lower(child) ||' ADD CONSTRAINT ' || constraint_name || ' UNIQUE (' || col || ')';
        EXECUTE command;
      ELSIF parent_constraint.contype = 'f' THEN -- foreign key
	    IF array_length(parent_constraint.confkey, 1) > 1 THEN
          RAISE EXCEPTION 'MULTICOLUMN FOREIGN KEY CONSTRAITS ARE NOT SUPPORTED %', parent_constraint.conname;
        END IF;
		SELECT relname INTO ref_table FROM pg_class WHERE oid = parent_constraint.confrelid;
		SELECT column_name INTO ref_col FROM information_schema.columns WHERE table_name = ref_table AND ordinal_position = parent_constraint.confkey[1];
        command := 'ALTER TABLE '  || child ||' ADD CONSTRAINT ' || constraint_name ||' FOREIGN KEY (' || col || ') REFERENCES ' || ref_table || '(' || ref_col || ')';  
		EXECUTE command;
      ELSE
        RAISE EXCEPTION 'UNSUPPORTED CONSTRAIT TYPE % %', parent_constraint.conkey, parent_constraint.conname; 
      END IF;
    END IF;
  END LOOP;
END $$;


CREATE OR REPLACE FUNCTION prevent_inserting() RETURNS TRIGGER LANGUAGE plpgsql AS $$
BEGIN
  RAISE EXCEPTION 'INSERTING IN TABLE IS FORBIDDEN';
  RETURN NULL;
END $$;

------------------------ TABLES AND CONSTRAINTS ---------------------------------------------------------
CREATE TABLE IF NOT EXISTS Persons
(
	PersonId serial PRIMARY KEY,
	Name text NOT NULL,
    Email text NOT NULL UNIQUE,
    Phone text NOT NULL UNIQUE
);

CREATE INDEX IF NOT EXISTS persons_name_idx ON Persons (Name);

CREATE TABLE IF NOT EXISTS Lectors
(
  Id serial PRIMARY KEY,
  PersonId integer NOT NULL REFERENCES Persons (PersonId)
);

CREATE TABLE IF NOT EXISTS Students
(
  Id serial PRIMARY KEY,
  PersonId integer NOT NULL REFERENCES Persons (PersonId)
);

CREATE OR REPLACE VIEW students_view AS SELECT Students.Id, Persons.* FROM Students INNER JOIN Persons ON Students.PersonId = Persons.PersonId; 

CREATE OR REPLACE VIEW lectors_view AS SELECT Lectors.Id, Persons.*  FROM Lectors INNER JOIN Persons ON Lectors.PersonId = Persons.PersonId; 

CREATE OR REPLACE FUNCTION  create_person(_table text, _name text, _email text, _phone text) RETURNS integer LANGUAGE plpgsql AS $$
DECLARE
  perId integer;
  retId integer;
  com text;
BEGIN
  INSERT INTO Persons (Name, Email, Phone) VALUES (_name, _email, _phone) RETURNING PersonId INTO perId;
  EXECUTE 'INSERT INTO ' || _table || '(PersonId) VALUES (' || perId::text || ') RETURNING Id' INTO retId;
  RETURN (SELECT retId);  
END $$;	

CREATE OR REPLACE FUNCTION  delete_person(_table text, _personId integer) RETURNS void LANGUAGE plpgsql AS $$
BEGIN
  EXECUTE 'DELETE FROM ' || _table ||  ' WHERE PersonId =' || _personId::text;
  DELETE FROM persons WHERE PersonId = _personId;  
END $$;


CREATE TABLE IF NOT EXISTS Courses
(
	Id serial PRIMARY KEY,
	Name text NOT NULL,
    LectorId integer NOT NULL REFERENCES Lectors (Id),
    UNIQUE (Name, LectorId)
);

CREATE TABLE IF NOT EXISTS Lections
(
     Id serial PRIMARY KEY,
     Name text,
     CourseId integer NOT NULL REFERENCES Courses (Id),
     Date timestamp,
     UNIQUE (CourseId, Name)
);

CREATE TABLE IF NOT EXISTS StudentsCourses
(
	Id serial PRIMARY KEY,
	StudentId integer NOT NULL REFERENCES Students (Id),
    CourseId  integer NOT NULL REFERENCES Courses (Id),
    UNIQUE (StudentId, CourseId)
);


CREATE TABLE IF NOT EXISTS Marks
(
     Id serial PRIMARY KEY,
     StudentCourseId integer NOT NULL REFERENCES StudentsCourses (Id),
     LectionId integer NOT NULL REFERENCES Lections (Id),
     Value smallint CHECK (value >= -1 AND Value <= 5),
	 UNIQUE (StudentCourseId, LectionId)

);


CREATE OR REPLACE FUNCTION check_studnetcourse_contains_lection() RETURNS trigger AS $$
DECLARE found integer; 
BEGIN
   found := 0;
   SELECT 1 INTO found FROM StudentsCourses AS SC INNER JOIN Lections AS L  ON  SC.CourseId = L.CourseId WHERE SC.Id = NEW.StudentCourseId AND L.Id = NEW.LectionId;
   IF found = 0 THEN
     RAISE EXCEPTION  USING ERRCODE = '23503'
                           ,MESSAGE = 'COURSE DOES NOT CONTAINS LECTION';
   END IF;
   RETURN NEW; 
END;
$$ LANGUAGE plpgsql;

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'check_course_contains_lection_trigger') THEN
		CREATE TRIGGER check_course_contains_lection_trigger
		BEFORE INSERT ON marks
		FOR EACH ROW
		EXECUTE PROCEDURE check_studnetcourse_contains_lection();
	END IF;
END $$;

---------------------------- Notifications ----------------------------------

CREATE TABLE IF NOT EXISTS Notifications
(
    Id serial PRIMARY KEY,
    PersonId integer NOT NULL REFERENCES Persons (PersonId),
    Message text NOT NULL,
    IsSent boolean NOT NULL
);

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_trigger WHERE tgname = 'prevent_notifications_inserting') THEN
        CREATE TRIGGER prevent_notifications_inserting BEFORE INSERT ON Notifications FOR EACH ROW EXECUTE PROCEDURE prevent_inserting();
    END IF;
END $$;

CREATE TABLE IF NOT EXISTS SmsMessages (
  Phone text NOT NULL
) INHERITS (Notifications);


CREATE TABLE IF NOT EXISTS EmailLetters (
  Email text NOT NULL
) INHERITS (Notifications);

DO $$
BEGIN
    PERFORM copy_parent_constraits('Notifications','SmsMessages');
    PERFORM copy_parent_constraits('Notifications','EmailLetters');
END $$;
