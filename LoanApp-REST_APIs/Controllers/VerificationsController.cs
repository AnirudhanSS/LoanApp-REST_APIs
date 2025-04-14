using LoanApp_REST_APIs.Models;
using LoanApp_REST_APIs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoanApp_REST_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationsController : ControllerBase
    {
        private readonly LoanManagementDbContext _context;

        public VerificationsController(LoanManagementDbContext context)
        {
            _context = context;
        }

        // GET: api/verifications/background
        [HttpGet("background")]
        [Authorize(Roles = "Admin,Officer")]
        public async Task<ActionResult<IEnumerable<VerificationViewModel>>> GetBackgroundVerifications()
        {
            var verifications = await _context.BackgroundVerificationAssignments
                .Join(_context.Users,
                    bva => bva.OfficerId,
                    u => u.UserId,
                    (bva, u) => new VerificationViewModel
                    {
                        AssignmentId = bva.AssignmentId,
                        RequestId = bva.RequestId,
                        OfficerId = bva.OfficerId,
                        OfficerName = u.Name,
                        Status = bva.Status,
                        VerificationDetails = bva.VerificationDetails,
                        AssignedAt = bva.AssignedAt
                    })
                .ToListAsync();

            return Ok(verifications);
        }

        // POST: api/verifications/background
        [HttpPost("background")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VerificationViewModel>> CreateBackgroundVerification([FromBody] VerificationCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var verification = new BackgroundVerificationAssignment
            {
                RequestId = model.RequestId,
                OfficerId = model.OfficerId,
                Status = "assigned",
                AssignedAt = DateTime.Now
            };

            _context.BackgroundVerificationAssignments.Add(verification);
            await _context.SaveChangesAsync();

            var result = await _context.BackgroundVerificationAssignments
                .Where(bva => bva.AssignmentId == verification.AssignmentId)
                .Join(_context.Users,
                    bva => bva.OfficerId,
                    u => u.UserId,
                    (bva, u) => new VerificationViewModel
                    {
                        AssignmentId = bva.AssignmentId,
                        RequestId = bva.RequestId,
                        OfficerId = bva.OfficerId,
                        OfficerName = u.Name,
                        Status = bva.Status,
                        VerificationDetails = bva.VerificationDetails,
                        AssignedAt = bva.AssignedAt
                    })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetBackgroundVerifications), new { id = result.AssignmentId }, result);
        }

        // PUT: api/verifications/background/1
        [HttpPut("background/{id}")]
        [Authorize(Roles = "Admin,Officer")]
        public async Task<ActionResult<VerificationViewModel>> UpdateBackgroundVerification(int id, [FromBody] VerificationUpdateViewModel model)
        {
            var verification = await _context.BackgroundVerificationAssignments
                .FirstOrDefaultAsync(bva => bva.AssignmentId == id);

            if (verification == null)
                return NotFound("Verification not found.");

            // Check if Officer is authorized
            if (User.IsInRole("Officer"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (verification.OfficerId != userId)
                    return Forbid("You can only update your own verifications.");
            }

            verification.Status = model.Status;
            verification.VerificationDetails = model.VerificationDetails;
            await _context.SaveChangesAsync();

            var result = await _context.BackgroundVerificationAssignments
                .Where(bva => bva.AssignmentId == id)
                .Join(_context.Users,
                    bva => bva.OfficerId,
                    u => u.UserId,
                    (bva, u) => new VerificationViewModel
                    {
                        AssignmentId = bva.AssignmentId,
                        RequestId = bva.RequestId,
                        OfficerId = bva.OfficerId,
                        OfficerName = u.Name,
                        Status = bva.Status,
                        VerificationDetails = bva.VerificationDetails,
                        AssignedAt = bva.AssignedAt
                    })
                .FirstOrDefaultAsync();

            return Ok(result);
        }

        // DELETE: api/verifications/background/1
        [HttpDelete("background/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBackgroundVerification(int id)
        {
            var verification = await _context.BackgroundVerificationAssignments
                .FirstOrDefaultAsync(bva => bva.AssignmentId == id);

            if (verification == null)
                return NotFound("Verification not found.");

            _context.BackgroundVerificationAssignments.Remove(verification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/verifications/loan
        [HttpGet("loan")]
        [Authorize(Roles = "Admin,Officer")]
        public async Task<ActionResult<IEnumerable<VerificationViewModel>>> GetLoanVerifications()
        {
            var verifications = await _context.LoanVerificationAssignments
                .Join(_context.Users,
                    lva => lva.OfficerId,
                    u => u.UserId,
                    (lva, u) => new VerificationViewModel
                    {
                        AssignmentId = lva.AssignmentId,
                        RequestId = lva.RequestId,
                        OfficerId = lva.OfficerId,
                        OfficerName = u.Name,
                        Status = lva.Status,
                        VerificationDetails = lva.VerificationDetails,
                        AssignedAt = lva.AssignedAt
                    })
                .ToListAsync();

            return Ok(verifications);
        }

        // POST: api/verifications/loan
        [HttpPost("loan")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<VerificationViewModel>> CreateLoanVerification([FromBody] VerificationCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var verification = new LoanVerificationAssignment
            {
                RequestId = model.RequestId,
                OfficerId = model.OfficerId,
                Status = "assigned",
                AssignedAt = DateTime.Now
            };

            _context.LoanVerificationAssignments.Add(verification);
            await _context.SaveChangesAsync();

            var result = await _context.LoanVerificationAssignments
                .Where(lva => lva.AssignmentId == verification.AssignmentId)
                .Join(_context.Users,
                    lva => lva.OfficerId,
                    u => u.UserId,
                    (lva, u) => new VerificationViewModel
                    {
                        AssignmentId = lva.AssignmentId,
                        RequestId = lva.RequestId,
                        OfficerId = lva.OfficerId,
                        OfficerName = u.Name,
                        Status = lva.Status,
                        VerificationDetails = lva.VerificationDetails,
                        AssignedAt = lva.AssignedAt
                    })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetLoanVerifications), new { id = result.AssignmentId }, result);
        }

        // PUT: api/verifications/loan/1
        [HttpPut("loan/{id}")]
        [Authorize(Roles = "Admin,Officer")]
        public async Task<ActionResult<VerificationViewModel>> UpdateLoanVerification(int id, [FromBody] VerificationUpdateViewModel model)
        {
            var verification = await _context.LoanVerificationAssignments
                .FirstOrDefaultAsync(lva => lva.AssignmentId == id);

            if (verification == null)
                return NotFound("Verification not found.");

            // Check if Officer is authorized
            if (User.IsInRole("Officer"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (verification.OfficerId != userId)
                    return Forbid("You can only update your own verifications.");
            }

            verification.Status = model.Status;
            verification.VerificationDetails = model.VerificationDetails;
            await _context.SaveChangesAsync();

            var result = await _context.LoanVerificationAssignments
                .Where(lva => lva.AssignmentId == id)
                .Join(_context.Users,
                    lva => lva.OfficerId,
                    u => u.UserId,
                    (lva, u) => new VerificationViewModel
                    {
                        AssignmentId = lva.AssignmentId,
                        RequestId = lva.RequestId,
                        OfficerId = lva.OfficerId,
                        OfficerName = u.Name,
                        Status = lva.Status,
                        VerificationDetails = lva.VerificationDetails,
                        AssignedAt = lva.AssignedAt
                    })
                .FirstOrDefaultAsync();

            return Ok(result);
        }

        // DELETE: api/verifications/loan/1
        [HttpDelete("loan/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLoanVerification(int id)
        {
            var verification = await _context.LoanVerificationAssignments
                .FirstOrDefaultAsync(lva => lva.AssignmentId == id);

            if (verification == null)
                return NotFound("Verification not found.");

            _context.LoanVerificationAssignments.Remove(verification);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}