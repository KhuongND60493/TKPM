using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.Entities;

namespace VstepPractice.Tests.Services.ScoreCalculation;

public static class TestData
{
    public static List<Question> CreateQuestions(
        SectionTypes sectionType,
        params (string Title, int QuestionCount)[] parts)
    {
        var questions = new List<Question>();
        var questionId = 1;
        var sectionId = 1;

        foreach (var (title, count) in parts)
        {
            var section = new SectionPart
            {
                Id = sectionId++,
                Title = title,
                SectionType = sectionType
            };

            for (int i = 0; i < count; i++)
            {
                questions.Add(new Question
                {
                    Id = questionId++,
                    Passage = section,
                    OrderNum = i,
                    Options = new List<QuestionOption>
                    {
                        new() { Id = questionId * 10 + 1, IsCorrect = true },
                        new() { Id = questionId * 10 + 2, IsCorrect = false },
                        new() { Id = questionId * 10 + 3, IsCorrect = false },
                        new() { Id = questionId * 10 + 4, IsCorrect = false }
                    }
                });
            }
        }

        return questions;
    }

    public static List<Answer> CreateAnswers(
        IEnumerable<Question> questions,
        decimal correctPercentage)
    {
        var answers = new List<Answer>();
        var questionsList = questions.ToList();
        var totalQuestions = questionsList.Count;
        var correctCount = (int)Math.Round(totalQuestions * correctPercentage);

        // Create correct answers first
        for (int i = 0; i < correctCount; i++)
        {
            var question = questionsList[i];
            var correctOption = question.Options.First(o => o.IsCorrect);
            answers.Add(new Answer
            {
                Id = question.Id,
                QuestionId = question.Id,
                QuestionOptionId = correctOption.Id,
                SelectedOption = correctOption
            });
        }

        // Create incorrect answers for remaining questions
        for (int i = correctCount; i < totalQuestions; i++)
        {
            var question = questionsList[i];
            var incorrectOption = question.Options.First(o => !o.IsCorrect);
            answers.Add(new Answer
            {
                Id = question.Id,
                QuestionId = question.Id,
                QuestionOptionId = incorrectOption.Id,
                SelectedOption = incorrectOption
            });
        }

        return answers;
    }
}
