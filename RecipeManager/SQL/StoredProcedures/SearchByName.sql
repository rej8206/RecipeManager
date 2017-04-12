CREATE PROCEDURE SearchByName
(
    -- Optional Filters for Dynamic Search
    @Name         VARCHAR = NULL,

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
    FROM Receipes 
    WHERE
        (@Name IS NULL OR RecepieName LIKE @Name)



END
GO