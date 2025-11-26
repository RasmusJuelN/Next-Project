using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class FixExistingDataSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing questions that have SortOrder = 0 (excluding seeded data)
            // Set SortOrder based on creation order (Id) within each questionnaire template
            migrationBuilder.Sql(@"
                WITH OrderedQuestions AS (
                    SELECT Id, 
                           ROW_NUMBER() OVER (PARTITION BY QuestionnaireTemplateFK ORDER BY Id) - 1 AS NewSortOrder
                    FROM QuestionnaireTemplateQuestion
                    WHERE SortOrder = 0 
                      AND Id NOT IN (-3, -2, -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)
                )
                UPDATE q 
                SET SortOrder = oq.NewSortOrder
                FROM QuestionnaireTemplateQuestion q
                INNER JOIN OrderedQuestions oq ON q.Id = oq.Id;
            ");

            // Update existing options that have SortOrder = 0 (excluding seeded data)
            // Set SortOrder based on creation order (Id) within each question
            migrationBuilder.Sql(@"
                WITH OrderedOptions AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (PARTITION BY QuestionFK ORDER BY Id) - 1 AS NewSortOrder
                    FROM QuestionnaireTemplateOption
                    WHERE SortOrder = 0
                      AND Id NOT IN (SELECT Id FROM QuestionnaireTemplateOption WHERE Id BETWEEN -9 AND 42 AND Id != 0)
                )
                UPDATE o
                SET SortOrder = oo.NewSortOrder  
                FROM QuestionnaireTemplateOption o
                INNER JOIN OrderedOptions oo ON o.Id = oo.Id;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Set SortOrder back to 0 for non-seeded data
            migrationBuilder.Sql(@"
                UPDATE QuestionnaireTemplateQuestion 
                SET SortOrder = 0 
                WHERE Id NOT IN (-3, -2, -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12);
            ");

            migrationBuilder.Sql(@"
                UPDATE QuestionnaireTemplateOption 
                SET SortOrder = 0 
                WHERE Id NOT IN (SELECT Id FROM QuestionnaireTemplateOption WHERE Id BETWEEN -9 AND 42 AND Id != 0);
            ");
        }
    }
}
