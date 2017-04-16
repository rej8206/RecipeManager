DELIMITER //
CREATE TRIGGER InsertRecipePartIngAndMeas BEFORE INSERT ON RecipeParts
FOR EACH ROW BEGIN
	IF NEW.MeasureName NOT IN (SELECT MeasureName FROM Measurements) THEN
		INSERT INTO Measurements VALUES(NEW.MeasureName, NULL);
	END IF;
	IF NEW.IngName NOT IN (SELECT IngName FROM Ingredients) THEN
		INSERT INTO Ingredients VALUES(NEW.IngName, NEW.MeasureName);
	END IF;
END;//
DELIMITER ;