using LoanApp_REST_APIs.Models;
using LoanApp_REST_APIs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_REST_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly LoanManagementDbContext _context;

        public FeedbackController(LoanManagementDbContext context)
        {
            _context = context;
        }

        // GET: api/feedback/questions
        [HttpGet("questions")]
        [Authorize(Roles = "Admin, Customer")]
        public async Task<ActionResult<IEnumerable<FeedbackQuestionViewModel>>> GetFeedbackQuestions()
        {
            var questions = await _context.FeedbackQuestions
                .Select(fq => new FeedbackQuestionViewModel
                {
                    QuestionId = fq.QuestionId,
                    QuestionText = fq.QuestionText,
                    CreatedAt = fq.CreatedAt
                })
                .ToListAsync();

            return Ok(questions);
        }

        // POST: api/feedback/questions
        [HttpPost("questions")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FeedbackQuestionViewModel>> CreateFeedbackQuestion([FromBody] FeedbackQuestionViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var question = new FeedbackQuestion
            {
                QuestionText = model.QuestionText,
                CreatedAt = DateTime.Now
            };

            _context.FeedbackQuestions.Add(question);
            await _context.SaveChangesAsync();

            var result = new FeedbackQuestionViewModel
            {
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                CreatedAt = question.CreatedAt
            };

            return CreatedAtAction(nameof(GetFeedbackQuestions), new { id = result.QuestionId }, result);
        }

        // PUT: api/feedback/questions/1
        [HttpPut("questions/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<FeedbackQuestionViewModel>> UpdateFeedbackQuestion(int id, [FromBody] FeedbackQuestionViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var question = await _context.FeedbackQuestions
                .FirstOrDefaultAsync(fq => fq.QuestionId == id);

            if (question == null)
                return NotFound("Feedback question not found.");

            question.QuestionText = model.QuestionText;
            await _context.SaveChangesAsync();

            var result = new FeedbackQuestionViewModel
            {
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                CreatedAt = question.CreatedAt
            };

            return Ok(result);
        }

        // GET: api/feedback
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<FeedbackSubmissionViewModel>>> GetFeedbackSubmissions()
        {
            var submissions = await _context.FeedbackSubmissions
                .Join(_context.Users,
                    fs => fs.CustomerId,
                    u => u.UserId,
                    (fs, u) => new { Submission = fs, Customer = u })
                .GroupJoin(_context.FeedbackAnswers,
                    fsu => fsu.Submission.SubmissionId,
                    fa => fa.SubmissionId,
                    (fsu, fa) => new { fsu.Submission, fsu.Customer, Answers = fa })
                .SelectMany(
                    fsu => fsu.Answers.DefaultIfEmpty(),
                    (fsu, fa) => new { fsu.Submission, fsu.Customer, Answer = fa })
                .Join(_context.FeedbackQuestions,
                    fsufa => fsufa.Answer != null ? fsufa.Answer.QuestionId : 0,
                    fq => fq.QuestionId,
                    (fsufa, fq) => new
                    {
                        fsufa.Submission.SubmissionId,
                        fsufa.Submission.CustomerId,
                        CustomerName = fsufa.Customer.Name,
                        fsufa.Submission.SubmissionDate,
                        AnswerId = fsufa.Answer != null ? fsufa.Answer.AnswerId : 0,
                        QuestionId = fsufa.Answer != null ? fsufa.Answer.QuestionId : 0,
                        QuestionText = fsufa.Answer != null ? fq.QuestionText : null,
                        AnswerText = fsufa.Answer != null ? fsufa.Answer.Answer : null
                    })
                .GroupBy(s => new { s.SubmissionId, s.CustomerId, s.CustomerName, s.SubmissionDate })
                .Select(g => new FeedbackSubmissionViewModel
                {
                    SubmissionId = g.Key.SubmissionId,
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    SubmissionDate = g.Key.SubmissionDate,
                    Answers = g.Where(a => a.AnswerId != 0)
                               .Select(a => new FeedbackAnswerViewModel
                               {
                                   AnswerId = a.AnswerId,
                                   QuestionId = a.QuestionId,
                                   QuestionText = a.QuestionText,
                                   Answer = a.AnswerText
                               })
                               .ToList()
                })
                .ToListAsync();

            return Ok(submissions);
        }

        // POST: api/feedback
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<FeedbackSubmissionViewModel>> CreateFeedback([FromBody] FeedbackCreateViewModel model)
        {
            // TODO: Verify model.CustomerId matches authenticated user

            if (!ModelState.IsValid || model.Answers == null || !model.Answers.Any())
                return BadRequest("Invalid feedback data.");

            var submission = new FeedbackSubmission
            {
                CustomerId = model.CustomerId,
                SubmissionDate = DateTime.Now
            };

            _context.FeedbackSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            foreach (var answer in model.Answers)
            {
                var feedbackAnswer = new FeedbackAnswer
                {
                    SubmissionId = submission.SubmissionId,
                    QuestionId = answer.QuestionId,
                    Answer = answer.Answer
                };
                _context.FeedbackAnswers.Add(feedbackAnswer);
            }

            await _context.SaveChangesAsync();

            var result = await _context.FeedbackSubmissions
                .Where(fs => fs.SubmissionId == submission.SubmissionId)
                .Join(_context.Users,
                    fs => fs.CustomerId,
                    u => u.UserId,
                    (fs, u) => new { Submission = fs, Customer = u })
                .GroupJoin(_context.FeedbackAnswers,
                    fsu => fsu.Submission.SubmissionId,
                    fa => fa.SubmissionId,
                    (fsu, fa) => new { fsu.Submission, fsu.Customer, Answers = fa })
                .SelectMany(
                    fsu => fsu.Answers.DefaultIfEmpty(),
                    (fsu, fa) => new { fsu.Submission, fsu.Customer, Answer = fa })
                .Join(_context.FeedbackQuestions,
                    fsufa => fsufa.Answer != null ? fsufa.Answer.QuestionId : 0,
                    fq => fq.QuestionId,
                    (fsufa, fq) => new
                    {
                        fsufa.Submission.SubmissionId,
                        fsufa.Submission.CustomerId,
                        CustomerName = fsufa.Customer.Name,
                        fsufa.Submission.SubmissionDate,
                        AnswerId = fsufa.Answer != null ? fsufa.Answer.AnswerId : 0,
                        QuestionId = fsufa.Answer != null ? fsufa.Answer.QuestionId : 0,
                        QuestionText = fsufa.Answer != null ? fq.QuestionText : null,
                        AnswerText = fsufa.Answer != null ? fsufa.Answer.Answer : null
                    })
                .GroupBy(s => new { s.SubmissionId, s.CustomerId, s.CustomerName, s.SubmissionDate })
                .Select(g => new FeedbackSubmissionViewModel
                {
                    SubmissionId = g.Key.SubmissionId,
                    CustomerId = g.Key.CustomerId,
                    CustomerName = g.Key.CustomerName,
                    SubmissionDate = g.Key.SubmissionDate,
                    Answers = g.Where(a => a.AnswerId != 0)
                               .Select(a => new FeedbackAnswerViewModel
                               {
                                   AnswerId = a.AnswerId,
                                   QuestionId = a.QuestionId,
                                   QuestionText = a.QuestionText,
                                   Answer = a.AnswerText
                               })
                               .ToList()
                })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetFeedbackSubmissions), new { id = result.SubmissionId }, result);
        }
    }
}