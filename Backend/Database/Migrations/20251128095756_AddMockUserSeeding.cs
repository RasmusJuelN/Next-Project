using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class AddMockUserSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Discriminator", "FullName", "Guid", "ParticipantIds", "Permissions", "PrimaryRole", "UserName" },
                values: new object[,]
                {
                    { -40, "TeacherModel", "Jackie Maddox", new Guid("0a14fb34-00d9-4854-a0ae-dfbe3a8595aa"), null, 24, "Teacher", "Jackie.Maddox" },
                    { -39, "TeacherModel", "Adeline Martin", new Guid("c4fd7938-d6b4-4e5b-aa38-b446a5a27384"), null, 24, "Teacher", "Adeline.Martin" },
                    { -38, "TeacherModel", "Hardy Cook", new Guid("088b71f2-a638-4f4e-83d3-08d12e0f520d"), null, 24, "Teacher", "Hardy.Cook" },
                    { -37, "TeacherModel", "Kirby Galloway", new Guid("e8795a6b-dc79-4c7d-a869-7cfae232439d"), null, 24, "Teacher", "Kirby.Galloway" },
                    { -36, "TeacherModel", "Mills Taylor", new Guid("4debbb11-51d3-4b1e-aeef-c671ecfd26d2"), null, 24, "Teacher", "Mills.Taylor" },
                    { -35, "TeacherModel", "Knox Whitley", new Guid("d354e86f-9604-451d-bfc7-267fe881a3a8"), null, 24, "Teacher", "Knox.Whitley" },
                    { -34, "TeacherModel", "Judith Chan", new Guid("bb2cac58-50bf-4cc0-9c46-77754c548d4b"), null, 24, "Teacher", "Judith.Chan" },
                    { -33, "TeacherModel", "Powell Mccoy", new Guid("7087977a-b3dc-4d31-80cc-8878197b9afe"), null, 24, "Teacher", "Powell.Mccoy" },
                    { -32, "TeacherModel", "Christie Pate", new Guid("7eaaf007-b414-40ad-b172-04afc3f31d02"), null, 24, "Teacher", "Christie.Pate" },
                    { -31, "TeacherModel", "Lancaster Booker", new Guid("351ab5d0-a12f-4951-9dd8-baddd4ab872b"), null, 24, "Teacher", "Lancaster.Booker" },
                    { -30, "TeacherModel", "Cruz Gutierrez", new Guid("41cdb0ce-8106-4dc8-8705-1bc4a8601c80"), null, 24, "Teacher", "Cruz.Gutierrez" },
                    { -29, "TeacherModel", "Stacie Woodard", new Guid("1429127b-67d5-4259-8f27-5836475c2254"), null, 24, "Teacher", "Stacie.Woodard" },
                    { -28, "TeacherModel", "Jamie Kirby", new Guid("6662cd03-9ccf-4846-87eb-89dcc003ab9c"), null, 24, "Teacher", "Jamie.Kirby" },
                    { -27, "TeacherModel", "Bentley Whitfield", new Guid("7b39758b-109c-4128-af6f-b123dba45063"), null, 24, "Teacher", "Bentley.Whitfield" },
                    { -26, "TeacherModel", "Santiago Le", new Guid("5c5a9a44-8fd8-43e3-9fb9-89842b441e34"), null, 24, "Teacher", "Santiago.Le" },
                    { -25, "TeacherModel", "Nicholson Coffey", new Guid("12e5a62e-865f-4c1c-94dd-e5cdaf47ae67"), null, 24, "Teacher", "Nicholson.Coffey" },
                    { -24, "TeacherModel", "Brianna Ewing", new Guid("9e7dc194-07f4-4f8f-b71a-e0f350b3c072"), null, 24, "Teacher", "Brianna.Ewing" },
                    { -23, "TeacherModel", "Hansen Kemp", new Guid("b6066162-a68e-4e15-841c-ce92c6c80e34"), null, 24, "Teacher", "Hansen.Kemp" },
                    { -22, "TeacherModel", "Howell Mcdonald", new Guid("b3cefcee-3e40-43ed-bd7d-00e4e296cdb6"), null, 24, "Teacher", "Howell.Mcdonald" },
                    { -21, "TeacherModel", "Goodman Powell", new Guid("2fd332b4-39b0-4d4b-9e52-04cd9eea2dd4"), null, 24, "Teacher", "Goodman.Powell" },
                    { -20, "StudentModel", "Consuelo Conrad", new Guid("8ecc7d4f-6126-44bd-9c89-cb475d478bde"), null, 16, "Student", "Consuelo.Conrad" },
                    { -19, "StudentModel", "Munoz Warren", new Guid("236d48e4-2b56-400c-83fe-824c469e22b0"), null, 16, "Student", "Munoz.Warren" },
                    { -18, "StudentModel", "Jannie Huber", new Guid("c6df5ff6-ebe2-4f92-9771-94f0953f6745"), null, 16, "Student", "Jannie.Huber" },
                    { -17, "StudentModel", "Finch Hayes", new Guid("32888c04-1fea-46cd-b767-59165f6bf74f"), null, 16, "Student", "Finch.Hayes" },
                    { -16, "StudentModel", "Prince Brady", new Guid("772c0c62-b0e3-4804-904d-b9645d6d0889"), null, 16, "Student", "Prince.Brady" },
                    { -15, "StudentModel", "Fran Gordon", new Guid("af6a88f1-c656-46a9-a001-77362ab47c58"), null, 16, "Student", "Fran.Gordon" },
                    { -14, "StudentModel", "Sellers Tate", new Guid("7949121f-188e-4541-9557-05004899fa82"), null, 16, "Student", "Sellers.Tate" },
                    { -13, "StudentModel", "Lee Morris", new Guid("2d50f55b-9065-4e64-b594-9c9f31c094a5"), null, 16, "Student", "Lee.Morris" },
                    { -12, "StudentModel", "Carlson Lara", new Guid("e34f1941-6bbd-4173-913f-db86d63981bc"), null, 16, "Student", "Carlson.Lara" },
                    { -11, "StudentModel", "Patti Hardin", new Guid("8a477829-4131-4187-824b-0e1b1e86747a"), null, 16, "Student", "Patti.Hardin" },
                    { -10, "StudentModel", "Marcy Bowen", new Guid("750d567f-7fb5-4797-8de6-1cce70bcee6a"), null, 16, "Student", "Marcy.Bowen" },
                    { -9, "StudentModel", "Mona Edwards", new Guid("dab46273-b302-4aea-8f7c-00278a6c79a1"), null, 16, "Student", "Mona.Edwards" },
                    { -8, "StudentModel", "White Duffy", new Guid("a93ba049-f2ab-4efd-8214-e3cb4c28c6a5"), null, 16, "Student", "White.Duffy" },
                    { -7, "StudentModel", "Flossie Francis", new Guid("a6723d38-9a5c-4268-9d97-eb4ab767e0cd"), null, 16, "Student", "Flossie.Francis" },
                    { -6, "StudentModel", "Fannie Bonner", new Guid("282a6274-c871-4bf2-ae5c-87a54675ac5b"), null, 16, "Student", "Fannie.Bonner" },
                    { -5, "StudentModel", "Matilda Hooper", new Guid("9a074fd3-563d-45a9-92d2-d9536077222d"), null, 16, "Student", "Matilda.Hooper" },
                    { -4, "StudentModel", "Dorothy Blake", new Guid("064913db-078f-4838-88ff-32a8f952fc64"), null, 16, "Student", "Dorothy.Blake" },
                    { -3, "StudentModel", "Faulkner Vincent", new Guid("99371161-f8c7-49ac-b4b1-5911e788eddf"), null, 16, "Student", "Faulkner.Vincent" },
                    { -2, "StudentModel", "Jenny Estes", new Guid("b5f38c7a-2703-4f67-9952-5117fa4c5a9c"), null, 16, "Student", "Jenny.Estes" },
                    { -1, "StudentModel", "Brigitte Cantrell", new Guid("7f8669b1-82ef-4992-b129-98816c8062ae"), null, 16, "Student", "Brigitte.Cantrell" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -40);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -39);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -38);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -37);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -36);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -35);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -34);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -33);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -32);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -31);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -30);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -29);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -28);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -27);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -26);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -25);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -24);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -23);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -22);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -21);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -20);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -19);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -18);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -17);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -16);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -15);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -14);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -13);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -12);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -11);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: -1);
        }
    }
}
