using AutoMapper;
using VstepPractice.API.Common.Enums;
using VstepPractice.API.Models.DTOs.AI;
using VstepPractice.API.Models.DTOs.Exams.Responses;
using VstepPractice.API.Models.DTOs.Questions.Responses;
using VstepPractice.API.Models.DTOs.SectionParts.Responses;
using VstepPractice.API.Models.DTOs.StudentAttempts.Responses;

using VstepPractice.API.Models.Entities;

namespace VstepPractice.API.Mapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Exam mappings
        CreateMap<Exam, ExamResponse>()
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => src.Title ?? string.Empty))
            .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description ?? string.Empty))
            .ForMember(dest => dest.Sections,
                opt => opt.MapFrom(src => src.SectionParts.Where(sp => sp.ParentId == null)
                    .OrderBy(sp => sp.OrderNum)));

        CreateMap<Exam, ExamStudentResponse>()
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => src.Title ?? string.Empty))
            .ForMember(dest => dest.Duration,
                opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description ?? string.Empty));

        // SectionPart mappings
        CreateMap<SectionPart, SectionPartResponse>()
            .ForMember(dest => dest.Title,
                opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Instructions,
                opt => opt.MapFrom(src => src.Instructions))
            .ForMember(dest => dest.Content,
                opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.SectionType,
                opt => opt.MapFrom(src => src.SectionType))
            .ForMember(dest => dest.Type,
                opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Children,
                opt => opt.MapFrom(src => src.Children.OrderBy(c => c.OrderNum)))
            .ForMember(dest => dest.Questions,
                opt => opt.MapFrom(src => src.Questions.OrderBy(q => q.OrderNum)));

        // Question mappings
        CreateMap<Question, QuestionResponse>()
            .ForMember(dest => dest.QuestionText,
                opt => opt.MapFrom(src => src.QuestionText ?? string.Empty))
            .ForMember(dest => dest.Points,
                opt => opt.MapFrom(src => src.Point))
            .ForMember(dest => dest.SectionType,
                opt => opt.MapFrom(src => src.Passage.SectionType))
            .ForMember(dest => dest.Options,
                opt => opt.MapFrom(src => src.Options.OrderBy(o => o.OrderNum)));

        // QuestionOption mappings
        CreateMap<QuestionOption, QuestionOptionResponse>()
            .ForMember(dest => dest.OptionText,
                opt => opt.MapFrom(src => src.Content ?? string.Empty))
            .ForMember(dest => dest.IsCorrect,
                opt => opt.MapFrom(src => src.IsCorrect));

        // StudentAttempt mappings
        CreateMap<StudentAttempt, AttemptResponse>()
            .ForMember(dest => dest.ExamTitle,
                opt => opt.MapFrom(src => src.Exam.Title ?? string.Empty))
            .ForMember(dest => dest.ExamDescription,
                opt => opt.MapFrom(src => src.Exam.Description ?? string.Empty))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Answers,
                opt => opt.MapFrom(src => src.Answers.OrderBy(a => a.Question.OrderNum)));
        CreateMap<StudentAttemptDetail, AttemptDetailResponse>();
        // StudentAttempt mappings to SummaryAttemptResponse
        CreateMap<StudentAttempt, SummaryAttemptResponse>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FinalScore,
                opt => opt.MapFrom(src => src.FinalScore))
            .ForMember(dest => dest.StartTime,
                opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime,
                opt => opt.MapFrom(src => src.EndTime!.Value));
        
        
        CreateMap<StudentAttempt, AttemptResultResponse>()
            .ForMember(dest => dest.ExamTitle,
                opt => opt.MapFrom(src => src.Exam.Title ?? string.Empty))
            .ForMember(dest => dest.StartTime,
                opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime,
                opt => opt.MapFrom(src => src.EndTime!.Value))
            .ForMember(dest => dest.Answers,
                opt => opt.MapFrom(src => src.Answers.OrderBy(a => a.Question.OrderNum)))
            .ForMember(dest => dest.SectionScores,
                opt => opt.MapFrom(src => new Dictionary<SectionTypes, decimal>()))
            .ForMember(dest => dest.FinalScore,
                opt => opt.Ignore());  // Sẽ được tính toán sau

        // Answer mappings
        CreateMap<Answer, AnswerResponse>()
            .ForMember(dest => dest.QuestionText,
                opt => opt.MapFrom(src => src.Question.QuestionText ?? string.Empty))
            .ForMember(dest => dest.PassageTitle,
                opt => opt.MapFrom(src => src.Question.Passage.Title))
            .ForMember(dest => dest.PassageContent,
                opt => opt.MapFrom(src => src.Question.Passage.Content))
            .ForMember(dest => dest.QuestionOptionId,
                opt => opt.MapFrom(src => src.QuestionOptionId))
            .ForMember(dest => dest.EssayAnswer,
                opt => opt.MapFrom(src => src.EssayAnswer))
            .ForMember(dest => dest.AiFeedback,
                opt => opt.MapFrom(src => src.AiFeedback))
            .ForMember(dest => dest.Score,
                opt => opt.MapFrom(src => src.Score))
            .ForMember(dest => dest.SectionType,
                opt => opt.MapFrom(src => src.Question.Passage.SectionType))
            .ForMember(dest => dest.IsCorrect,
                opt => opt.MapFrom(src =>
                    src.Question.Passage.SectionType != SectionTypes.Writing &&
                    src.SelectedOption != null &&
                    src.SelectedOption.IsCorrect))
            .ForMember(dest => dest.WritingScore,
                opt => opt.Ignore())
            .ForMember(dest => dest.SpeakingScore, opt => opt.Ignore()); ; // We'll set this separately when needed


        CreateMap<SpeakingAssessment, SpeakingScoreDetails>();

        CreateMap<SpeakingAssessmentResponse, SpeakingAssessment>()
            .ForMember(dest => dest.Pronunciation, opt => opt.MapFrom(src => src.PronScore))
            .ForMember(dest => dest.Fluency, opt => opt.MapFrom(src => src.FluencyScore))
            .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.AccuracyScore))
            .ForMember(dest => dest.Prosody, opt => opt.MapFrom(src => src.ProsodyScore))
            .ForMember(dest => dest.Vocabulary, opt => opt.MapFrom(src => src.VocabularyScore))
            .ForMember(dest => dest.Grammar, opt => opt.MapFrom(src => src.GrammarScore))
            .ForMember(dest => dest.TopicScore, opt => opt.MapFrom(src => src.TopicScore))
            .ForMember(dest => dest.DetailedFeedback, opt => opt.MapFrom(src => src.DetailedFeedback))
            .ForMember(dest => dest.TranscribedText, opt => opt.MapFrom(src => src.RecognizedText))
            .ForMember(dest => dest.WordDetails, opt => opt.MapFrom(src => src.Words))
            .ForMember(dest => dest.AssessedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Add mappings for WordDetail if needed
        CreateMap<WordDetail, WordDetail>();
        CreateMap<PhonemeDetail, PhonemeDetail>();

        CreateMap<WritingAssessmentResponse, WritingAssessment>()
            .ForMember(dest => dest.TaskAchievement, opt => opt.MapFrom(src => src.TaskAchievement))
            .ForMember(dest => dest.CoherenceCohesion, opt => opt.MapFrom(src => src.CoherenceCohesion))
            .ForMember(dest => dest.LexicalResource, opt => opt.MapFrom(src => src.LexicalResource))
            .ForMember(dest => dest.GrammarAccuracy, opt => opt.MapFrom(src => src.GrammarAccuracy))
            .ForMember(dest => dest.DetailedFeedback, opt => opt.MapFrom(src => src.DetailedFeedback))
            .ForMember(dest => dest.AssessedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}