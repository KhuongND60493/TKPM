using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using VstepPractice.API.Data;
using VstepPractice.API.Models.Entities;
using VstepPractice.API.Repositories.Interfaces;

namespace VstepPractice.API.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    private IExamRepository? _examRepository;
    private IQuestionOptionRepository? _questionOptionRepository;
    private IStudentAttemptRepository? _studentAttemptRepository;
    private IStudentAttemptDetailRepository? _studentAttemptDetailRepository;
    private IAnswerRepository? _answerRepository;
    private IQuestionRepository? _questionRepository;
    private IWritingAssessmentRepository? _writingAssessmentRepository;
    private ISpeakingAssessmentRepository? _speakingAssessmentRepository;
    private ISectionPartRepository? _sectionPartRepository;

    public UnitOfWork(
        ApplicationDbContext context) // Inject các dependencies cần thiết
    {
        _context = context;
        // Khởi tạo các repositories khi cần
        _examRepository = new ExamRepository(_context);
    }

    public IExamRepository ExamRepository => _examRepository ??= new ExamRepository(_context);

    public IQuestionOptionRepository QuestionOptions => _questionOptionRepository ??= new QuestionOptionRepository(_context);

    public IStudentAttemptRepository StudentAttemptRepository =>
        _studentAttemptRepository ??= new StudentAttemptRepository(_context);
    public IStudentAttemptDetailRepository StudentAttemptDetailRepository =>
        _studentAttemptDetailRepository ??= new StudentAttemptDetailRepository(_context);
    public IAnswerRepository AnswerRepository =>
        _answerRepository ??= new AnswerRepository(_context);

    public IQuestionRepository QuestionRepository => _questionRepository ??= new QuestionRepository(_context);

    public IWritingAssessmentRepository WritingAssessmentRepository =>
        _writingAssessmentRepository ??= new WritingAssessmentRepository(_context);

    public ISpeakingAssessmentRepository SpeakingAssessmentRepository =>
        _speakingAssessmentRepository ??= new SpeakingAssessmentRepository(_context);

    public ISectionPartRepository SectionPartRepository => _sectionPartRepository ??= new SectionPartRepository(_context);
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
            _transaction?.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
