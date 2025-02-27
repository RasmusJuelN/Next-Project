using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddseedingofdefaultQuestionnaireTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "QuestionnaireTemplate",
                columns: new[] { "Id", "Description", "IsLocked", "Title" },
                values: new object[] { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), "Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD", false, "Evaluering af SKP-elever" });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplateQuestion",
                columns: new[] { "Id", "AllowCustom", "Prompt", "QuestionnaireTemplateFK" },
                values: new object[,]
                {
                    { 1, false, "Indlæringsevne", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 2, false, "Kreativitet og selvstændighed", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 3, false, "Arbejdsindsats", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 4, false, "Orden og omhyggelighed", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 5, false, "N/A", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 6, false, "N/A", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 7, false, "N/A", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 8, false, "Mødestabilitet", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 9, false, "Sygdom", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 10, false, "Fravær", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 11, false, "Praktikpladssøgning", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") },
                    { 12, false, "Synlighed", new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6") }
                });

            migrationBuilder.InsertData(
                table: "QuestionnaireTemplateOption",
                columns: new[] { "Id", "DisplayText", "OptionValue", "QuestionFK" },
                values: new object[,]
                {
                    { 1, "Viser lidt eller ingen forståelse for arbejdsopgaverne.", 1, 1 },
                    { 2, "Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden.", 2, 1 },
                    { 3, "Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.", 3, 1 },
                    { 4, "Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.", 4, 1 },
                    { 5, "Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.", 5, 1 },
                    { 6, "Viser intet initiativ. Er passiv, uinteresseret og uselvstændig.", 1, 2 },
                    { 7, "Viser ringe initiativ. Kommer ikke selv med løsningsforslag. Viser ingen interesse i at tilrettelægge eget arbejde.", 2, 2 },
                    { 8, "Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.", 3, 2 },
                    { 9, "Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.", 4, 2 },
                    { 10, "Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.", 5, 2 },
                    { 11, "Uacceptabel", 1, 3 },
                    { 12, "Under middel", 2, 3 },
                    { 13, "Middel", 3, 3 },
                    { 14, "Over middel", 4, 3 },
                    { 15, "Særdeles god", 5, 3 },
                    { 16, "Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.", 1, 4 },
                    { 17, "Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.", 2, 4 },
                    { 18, "Påpasselighed og omhyggelighed middel. Rimelig god orden.", 3, 4 },
                    { 19, "Meget påpasselig både i praktik og teori. God orden.", 4, 4 },
                    { 20, "I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.", 5, 4 },
                    { 21, "N/A", 1, 5 },
                    { 22, "N/A", 1, 6 },
                    { 23, "N/A", 1, 7 },
                    { 24, "Du møder ikke hver dag til tiden.", 1, 8 },
                    { 25, "Du møder næsten hver dag til tiden.", 2, 8 },
                    { 26, "Du møder hver dag til tiden.", 3, 8 },
                    { 27, "Du melder ikke afbud ved sygdom.", 1, 9 },
                    { 28, "Du melder, for det meste afbud, når du er syg.", 2, 9 },
                    { 29, "Du melder afbud, når du er syg.", 3, 9 },
                    { 30, "Du har et stort fravær.", 1, 10 },
                    { 31, "Du har noget fravær.", 2, 10 },
                    { 32, "Du har stort set ingen fravær.", 3, 10 },
                    { 33, "Du har ingen fravær.", 4, 10 },
                    { 34, "Du søger ingen praktikpladser.", 1, 11 },
                    { 35, "Du ved, at du skal søge alle relevante praktikpladser, men det kniber med handlingen.", 2, 11 },
                    { 36, "Du søger alle relevante praktikpladser, men skal have hjælp til at søge praktikpladser, der ligger længere væk end i din bopælskommune.", 3, 11 },
                    { 37, "Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune.", 4, 11 },
                    { 38, "Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune. Du søger også praktikplads inden for en anden uddannelse, som dit GF giver adgang til.", 5, 11 },
                    { 39, "Du har ikke en synlig profil på praktikpladsen.dk.", 1, 12 },
                    { 40, "Du skal ofte påmindes om at synliggøre din profil på praktikpladsen.dk.", 2, 12 },
                    { 41, "Du har altid en synlig, men ikke opdateret profil på praktikpladsen.dk.", 3, 12 },
                    { 42, "Du har altid en opdateret og synlig profil på praktikpladsen.dk.", 4, 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateOption",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplateQuestion",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "QuestionnaireTemplate",
                keyColumn: "Id",
                keyValue: new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"));
        }
    }
}
