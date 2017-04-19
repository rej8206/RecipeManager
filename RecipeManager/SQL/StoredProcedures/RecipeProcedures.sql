DROP FUNCTION IF EXISTS CreateRecipe;

DELIMITER //

CREATE PROCEDURE CreateRecipe(
	r_name			VARCHAR(100),
	r_instructions	TEXT,
	r_image			VARCHAR(200),
	r_servings		INT,
	r_minutesToMake	INT)
BEGIN
	DECLARE r_id			INT;
	DECLARE r_sourceName	VARCHAR(50);

	SELECT MIN(RecipeId) INTO r_id
	FROM (
		SELECT RecipeId
		FROM Recipes
			UNION
		SELECT 0 AS RecipeId) AS UsedIds
	WHERE UsedIds.RecipeId + 1 NOT IN (
		SELECT RecipeId
		FROM Recipes);
	SET r_id = r_id + 1;	

	SELECT Username INTO r_sourceName
	FROM CurrentUser;

	INSERT INTO Recipes VALUES (
		r_id,
		r_name,
		r_instructions,
		r_image,
		r_servings,
		r_sourceName,
		r_minutesToMake);
END;

DELIMITER ;