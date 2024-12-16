using Microsoft.EntityFrameworkCore;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Data;

public static class DbSeedData
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            // Check if database has been seeded
            if (await context.Exams.AnyAsync())
            {
                logger.LogInformation("Database already seeded");
                return;
            }

            // Create admin user
            var admin = new User
            {
                Email = "admin@example.com",
                Password = "assddsaas",
                FirstName = "Admin",
                LastName = "User",
                Role = UserRoles.Admin
            };
            context.Users.Add(admin);
            await context.SaveChangesAsync();

            // Create exam
            var exam = new Exam
            {
                Title = "VSTEP B2 Practice Test",
                Description = "Full practice test for VSTEP B2 certification",
                UserId = admin.Id
            };
            context.Exams.Add(exam);
            await context.SaveChangesAsync();

            // Listening Section
            var listeningSection = new SectionPart
            {
                Title = "Listening",
                Instructions = "Listen to the audio and answer the questions",
                SectionType = SectionTypes.Listening,
                Type = SectionPartTypes.Section,
                OrderNum = 1,
                ExamId = exam.Id
            };
            context.SectionParts.Add(listeningSection);
            await context.SaveChangesAsync();

            // Listening Parts
            for (int i = 1; i <= 2; i++)
            {
                var part = new SectionPart
                {
                    Title = $"Listening Part {i}",
                    Content = $"Audio content for part {i}",
                    SectionType = SectionTypes.Listening,
                    Type = SectionPartTypes.Part,
                    OrderNum = i,
                    ExamId = exam.Id,
                    ParentId = listeningSection.Id
                };
                context.SectionParts.Add(part);
                await context.SaveChangesAsync();

                // Add questions for each part
                for (int j = 1; j <= 2; j++)
                {
                    var question = new Question
                    {
                        PassageId = part.Id,
                        QuestionText = $"Listening Question {j} for Part {i}",
                        Point = 1,
                        OrderNum = j
                    };
                    context.Questions.Add(question);
                    await context.SaveChangesAsync();

                    // Add 4 options for each question
                    for (int k = 1; k <= 4; k++)
                    {
                        var option = new QuestionOption
                        {
                            QuestionId = question.Id,
                            Content = $"Option {k}",
                            IsCorrect = k == 1, // First option is correct
                            OrderNum = k
                        };
                        context.QuestionOptions.Add(option);
                    }
                }
            }

            // Reading Section
            var readingSection = new SectionPart
            {
                Title = "Reading",
                Instructions = "Read the passages and answer the questions",
                SectionType = SectionTypes.Reading,
                Type = SectionPartTypes.Section,
                OrderNum = 2,
                ExamId = exam.Id
            };
            context.SectionParts.Add(readingSection);
            await context.SaveChangesAsync();

            // Reading Parts
            for (int i = 1; i <= 2; i++)
            {
                var part = new SectionPart
                {
                    Title = $"Reading Part {i}",
                    Content = $"Reading passage content for part {i}",
                    SectionType = SectionTypes.Reading,
                    Type = SectionPartTypes.Part,
                    OrderNum = i,
                    ExamId = exam.Id,
                    ParentId = readingSection.Id
                };
                context.SectionParts.Add(part);
                await context.SaveChangesAsync();

                // Add questions for each part
                for (int j = 1; j <= 2; j++)
                {
                    var question = new Question
                    {
                        PassageId = part.Id,
                        QuestionText = $"Reading Question {j} for Part {i}",
                        Point = 1,
                        OrderNum = j
                    };
                    context.Questions.Add(question);
                    await context.SaveChangesAsync();

                    // Add 4 options for each question
                    for (int k = 1; k <= 4; k++)
                    {
                        var option = new QuestionOption
                        {
                            QuestionId = question.Id,
                            Content = $"Option {k}",
                            IsCorrect = k == 1, // First option is correct
                            OrderNum = k
                        };
                        context.QuestionOptions.Add(option);
                    }
                }
            }

            // Writing Section
            var writingSection = new SectionPart
            {
                Title = "Writing",
                Instructions = "Complete both writing tasks",
                SectionType = SectionTypes.Writing,
                Type = SectionPartTypes.Section,
                OrderNum = 3,
                ExamId = exam.Id
            };
            context.SectionParts.Add(writingSection);
            await context.SaveChangesAsync();

            // Writing Task 1
            var writingTask1 = new SectionPart
            {
                Title = "Email Writing",
                Instructions = "You should spend about 20 minutes on this task.",
                Content = @"You received an email from your English friend, Jane. She asked you for some information about one of your friends. Read part of her email below.

I've just got an email from your friend, An. She said she's going to take a course in London this summer. She asked if she could stay with my family until she could find an apartment. Can you tell me a bit about her (things like her personality, hobbies and interests, and her current work or study if possible)? I want to see if she will fit in with my family.",
                SectionType = SectionTypes.Writing,
                Type = SectionPartTypes.Passage,
                OrderNum = 1,
                ExamId = exam.Id,
                ParentId = writingSection.Id
            };
            context.SectionParts.Add(writingTask1);
            await context.SaveChangesAsync();

            var writingQuestion1 = new Question
            {
                PassageId = writingTask1.Id,
                QuestionText = "Write an email responding to Jane. You should write at least 120 words. You do not need to include your name or addresses. Your response will be evaluated in terms of Task Fulfillment, Organization, Vocabulary and Grammar.",
                Point = 10,
                OrderNum = 1
            };
            context.Questions.Add(writingQuestion1);

            // Writing Task 2
            var writingTask2 = new SectionPart
            {
                Title = "Essay Writing",
                Instructions = "You should spend about 40 minutes on this task.",
                Content = @"Tourism has become one of the fastest growing industries in the world. Millions of people today are travelling farther and farther throughout the year. Some people argue that the development of tourism has had negative effects on local communities; others think that its influences are positive.",
                SectionType = SectionTypes.Writing,
                Type = SectionPartTypes.Passage,
                OrderNum = 2,
                ExamId = exam.Id,
                ParentId = writingSection.Id
            };
            context.SectionParts.Add(writingTask2);
            await context.SaveChangesAsync();

            var writingQuestion2 = new Question
            {
                PassageId = writingTask2.Id,
                QuestionText = "Write an essay to an educated reader to discuss the effects of tourism on local communities. Include reasons and any relevant examples to support your answer. You should write at least 250 words. Your response will be evaluated in terms of Task Fulfillment, Organization, Vocabulary and Grammar.",
                Point = 10,
                OrderNum = 1
            };
            context.Questions.Add(writingQuestion2);

            // Speaking Section
            var speakingSection = new SectionPart
            {
                Title = "Speaking",
                Instructions = "Complete both speaking tasks",
                SectionType = SectionTypes.Speaking,
                Type = SectionPartTypes.Section,
                OrderNum = 4,
                ExamId = exam.Id
            };
            context.SectionParts.Add(speakingSection);
            await context.SaveChangesAsync();

            // Speaking Part 1
            var speakingPart1 = new SectionPart
            {
                Title = "Social Interaction",
                Instructions = "This part will take 3 minutes",
                Content = "Let's talk about your free time activities.",
                SectionType = SectionTypes.Speaking,
                Type = SectionPartTypes.Part,
                OrderNum = 1,
                ExamId = exam.Id,
                ParentId = speakingSection.Id
            };
            context.SectionParts.Add(speakingPart1);
            await context.SaveChangesAsync();

            var speakingQuestion1 = new Question
            {
                PassageId = speakingPart1.Id,
                QuestionText = "What do you often do in your free time?",
                Point = 10,
                OrderNum = 1
            };
            context.Questions.Add(speakingQuestion1);

            var speakingQuestion2 = new Question
            {
                PassageId = speakingPart1.Id,
                QuestionText = "Do you watch TV? If no, why not? If yes, which TV channel do you like best? Why?",
                Point = 10,
                OrderNum = 2
            };
            context.Questions.Add(speakingQuestion2);

            // Speaking Part 2
            var speakingPart2 = new SectionPart
            {
                Title = "Solution Discussion",
                Instructions = "This part will take 4 minutes",
                Content = "Situation: A group of people is planning a trip from Danang to Hanoi. Three means of transport are suggested: by train, by plane, and by coach.",
                SectionType = SectionTypes.Speaking,
                Type = SectionPartTypes.Part,
                OrderNum = 2,
                ExamId = exam.Id,
                ParentId = speakingSection.Id
            };
            context.SectionParts.Add(speakingPart2);
            await context.SaveChangesAsync();

            var speakingQuestion3 = new Question
            {
                PassageId = speakingPart2.Id,
                QuestionText = "Which means of transport do you think is the best choice?",
                Point = 10,
                OrderNum = 1
            };
            context.Questions.Add(speakingQuestion3);

            await context.SaveChangesAsync();
            logger.LogInformation("Database seeded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }
}