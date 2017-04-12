DROP FUNCTION IF EXISTS ValidateUser;
DROP PROCEDURE IF EXISTS CreateUser;
DROP PROCEDURE IF EXISTS ChangeUserPassword;

DELIMITER //

CREATE FUNCTION ValidateUser(
	u_name VARCHAR(40), pwd VARCHAR(100))
	RETURNS BOOL
BEGIN
	DECLARE storedId		INT;
	DECLARE storedSalt		VARCHAR(40);
	DECLARE thisHash		VARCHAR(40);
	DECLARE storedHash		VARCHAR(40);
	DECLARE result			BOOL			DEFAULT FALSE;

	SELECT UserId, PassHash, Salt INTO storedId, storedHash, storedSalt FROM Users WHERE Username = u_name;
	SET thisHash = SHA(CONCAT(pwd, storedSalt));
	IF thisHash = storedHash THEN
		SET result = TRUE;
		SET @currentUser = storedId;
	ELSE
		SET result = FALSE;
		SET @currentUser = NULL;
	END IF;
	CALL CreateUserTables;
	RETURN result;
END; //

CREATE PROCEDURE CreateUser(
	IN u_name VARCHAR(40), IN pwd VARCHAR(100))
BEGIN
	DECLARE salt			VARCHAR(40);
	DECLARE passHash		VARCHAR(40);

	SET salt = SHA(RAND()); -- 40 random characters
	SET passHash = SHA(CONCAT(pwd, salt));
	INSERT INTO Users (
		Username,
		PassHash,
		Salt)
	VALUES (
		u_name,
		passHash,
		salt);
END; //

CREATE PROCEDURE ChangeUserPassword(
	IN u_name VARCHAR(40), IN oldPwd VARCHAR(100), IN newPwd VARCHAR(100))
BEGIN
	DECLARE isValid			BOOL;
	DECLARE newSalt			VARCHAR(40);
	DECLARE newPassHash		VARCHAR(40);

	SELECT ValidateUser(u_name, oldPwd) INTO isValid;
	IF isValid THEN
		SET newSalt = SHA(RAND()); -- 40 random characters
		SET newPassHash = SHA(CONCAT(newPwd, newSalt));

		UPDATE Users
		SET PassHash = newPassHash, Salt = newSalt 
		WHERE UserId = @currentUser;
	END IF
END; //

DELIMITER ;