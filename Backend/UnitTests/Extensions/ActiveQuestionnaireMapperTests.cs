//using Database.Models;
//using Microsoft.AspNetCore.Mvc.ApplicationModels;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Database.DTO.ActiveQuestionnaire;
//using Database.Extensions;



//namespace UnitTests.Extensions
//{
//    public class ActiveQuestionnaireMapperTests
//    {
//        private static StudentModel CreateStudent(int id, string fullName)
//        {
//            return new StudentModel
//            {
//                Id = id,
//                Guid = Guid.NewGuid(),
//                FullName = fullName,
//                UserName = fullName.Replace(" ", ".").ToLower(),
//                PrimaryRole = "Student",
//                Permissions = new List<string>()
//            };
//        }

//        private static TeacherModel CreateTeacher(int id, string fullName)
//        {
//            return new TeacherModel
//            {
//                Id = id,
//                Guid = Guid.NewGuid(),
//                FullName = fullName,
//                UserName = fullName.Replace(" ", ".").ToLower(),
//                PrimaryRole = "Teacher",
//                Permissions = new List<string>()
//            };
//        }

//        private static QuestionModel CreateQuestion(Guid id, string prompt)
//        {
//            return new QuestionModel
//            {
//                Id = id,
//                Prompt = prompt,
//                Options = new List<OptionModel>
//                {
//                    new OptionModel { Id = Guid.NewGuid(), DisplayText = "Yes" },
//                    new OptionModel { Id = Guid.NewGuid(), DisplayText = "No" }
//                }
//            };
//        }

//        [Fact]
//        public void ToBaseDto_ShouldMapBasicProperties()
//        {
//            // Arrange
//            var student = CreateStudent(1, "John Student");
//            var teacher = CreateTeacher(2, "Alice Teacher");

//            var model = new ActiveQuestionnaireModel
//            {
//                Id = Guid.NewGuid(),
//                Title = "Sample Questionnaire",
//                Description = "Description text",
//                ActivatedAt = DateTime.UtcNow,
//                Student = student,
//                Teacher = teacher,
//                StudentCompletedAt = DateTime.UtcNow.AddMinutes(-10),
//                TeacherCompletedAt = DateTime.UtcNow
//            };

//            // Act
//            var dto = model.ToBaseDto();

//            // Assert
//            Assert.Equal(model.Id, dto.Id);
//            Assert.Equal(model.Title, dto.Title);
//            Assert.Equal(model.Description, dto.Description);
//            Assert.Equal(student.FullName, dto.Student.FullName);
//            Assert.Equal(teacher.FullName, dto.Teacher.FullName);
//            Assert.Equal(model.StudentCompletedAt, dto.StudentCompletedAt);
//            Assert.Equal(model.TeacherCompletedAt, dto.TeacherCompletedAt);
//        }

//        [Fact]
//        public void ToDto_ShouldMapQuestionsFromTemplate()
//        {
//            // Arrange
//            var student = CreateStudent(1, "Student A");
//            var teacher = CreateTeacher(2, "Teacher A");

//            var questions = new List<QuestionModel>
//            {
//                CreateQuestion(Guid.NewGuid(), "How are you?"),
//                CreateQuestion(Guid.NewGuid(), "Favorite subject?")
//            };

//            var template = new QuestionnaireTemplateModel
//            {
//                Id = Guid.NewGuid(),
//                Title = "Template 1",
//                Questions = questions
//            };

//            var model = new ActiveQuestionnaireModel
//            {
//                Id = Guid.NewGuid(),
//                Title = "Q1",
//                Description = "Testing template mapping",
//                ActivatedAt = DateTime.UtcNow,
//                Student = student,
//                Teacher = teacher,
//                QuestionnaireTemplate = template
//            };

//            // Act
//            var dto = model.ToDto();

//            // Assert
//            Assert.Equal(model.Id, dto.Id);
//            Assert.Equal(2, dto.Questions.Count);
//            Assert.Contains(dto.Questions, q => q.Prompt == "How are you?");
//            Assert.Contains(dto.Questions, q => q.Prompt == "Favorite subject?");
//        }

//        [Fact]
//        public void ToFullResponseAll_ShouldMapAnswersCorrectly()
//        {
//            // Arrange
//            var question = CreateQuestion(Guid.NewGuid(), "Favorite color?");
//            var option = question.Options.First();

//            var studentAnswer = new ActiveQuestionnaireStudentResponseModel
//            {
//                Question = question,
//                Option = option
//            };

//            var teacherAnswer = new ActiveQuestionnaireTeacherResponseModel
//            {
//                Question = question,
//                CustomResponse = "Blue"
//            };

//            var model = new ActiveQuestionnaireModel
//            {
//                Id = Guid.NewGuid(),
//                Title = "Response Test",
//                Description = "Checking answers",
//                Student = CreateStudent(1, "Student A"),
//                Teacher = CreateTeacher(2, "Teacher B"),
//                StudentCompletedAt = DateTime.UtcNow.AddHours(-1),
//                TeacherCompletedAt = DateTime.UtcNow,
//                StudentAnswers = new List<ActiveQuestionnaireStudentResponseModel> { studentAnswer },
//                TeacherAnswers = new List<ActiveQuestionnaireTeacherResponseModel> { teacherAnswer }
//            };

//            // Act
//            var dto = model.ToFullResponseAll();

//            // Assert
//            Assert.Equal("Favorite color?", dto.Answers[0].Question);
//            Assert.Equal("Yes", dto.Answers[0].StudentResponse);
//            Assert.Equal("Blue", dto.Answers[0].TeacherResponse);
//            Assert.False(dto.Answers[0].IsStudentResponseCustom);
//            Assert.True(dto.Answers[0].IsTeacherResponseCustom);
//        }

//        [Fact]
//        public void ToFullStudentRespondsDate_ShouldMapStudentAnswers()
//        {
//            // Arrange
//            var question = CreateQuestion(Guid.NewGuid(), "How old are you?");
//            var option = question.Options.First();

//            var studentAnswer = new ActiveQuestionnaireStudentResponseModel
//            {
//                Question = question,
//                Option = option
//            };

//            var model = new ActiveQuestionnaireModel
//            {
//                Id = Guid.NewGuid(),
//                Title = "Student Response",
//                Description = "Student answers only",
//                Student = CreateStudent(1, "Student A"),
//                StudentCompletedAt = DateTime.UtcNow,
//                StudentAnswers = new List<ActiveQuestionnaireStudentResponseModel> { studentAnswer }
//            };

//            // Act
//            var dto = model.ToFullStudentRespondsDate();

//            // Assert
//            Assert.Equal("How old are you?", dto.Answers[0].Question);
//            Assert.Equal("Yes", dto.Answers[0].StudentResponse);
//            Assert.False(dto.Answers[0].IsStudentResponseCustom);
//        }
//    }
//}
