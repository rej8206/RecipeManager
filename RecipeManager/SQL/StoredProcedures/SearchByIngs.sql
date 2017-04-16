CREATE PROCEDURE SearchByIng
(
    -- Optional Filters for Dynamic Search
    @Ing1          CHAR = NULL, 
    
)
AS
BEGIN
    SET NOCOUNT ON
   
    SELECT
	   RecipeName,
	   Description,
	   Instruction,
	   Yield,
	   Image,
	   Culture,
    FROM Receipes JOIN RecepieParts ON Recipes.ReceipeId =RecepieParts.ReceipeId
    WHERE
        (@Ing1 IS NULL OR RecepieParts.IngName LIKE @Ing1)
   
   
         
END
GO