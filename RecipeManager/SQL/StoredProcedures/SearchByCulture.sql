DROP PROCEDURE IF EXISTS SearchByCulture;


CREATE PROCEDURE SearchByCulture
(

    @Culture         VARCHAR = NULL, 
    
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
        (@Culture IS NULL OR Culture LIKE @Culture)
   
   
         
END
GO
