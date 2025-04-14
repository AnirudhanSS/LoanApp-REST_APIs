using LoanApp_REST_APIs.Models;
using LoanApp_REST_APIs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_REST_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanRequestsController : ControllerBase
    {
        private readonly LoanManagementDbContext _context;

        public LoanRequestsController(LoanManagementDbContext context)
        {
            _context = context;
        }

        // GET: api/loanrequests
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<LoanRequestViewModel>>> GetAllLoanRequests()
        {
            var loanRequests = await _context.LoanRequests
                .Join(_context.Users,
                    lr => lr.CustomerId,
                    uc => uc.UserId,
                    (lr, uc) => new { LoanRequest = lr, Customer = uc })
                .GroupJoin(_context.Users,
                    lruc => lruc.LoanRequest.AssignedOfficerId,
                    uo => uo.UserId,
                    (lruc, uo) => new { lruc.LoanRequest, lruc.Customer, Officers = uo })
                .SelectMany(
                    lruc => lruc.Officers.DefaultIfEmpty(),
                    (lruc, uo) => new LoanRequestViewModel
                    {
                        RequestId = lruc.LoanRequest.RequestId,
                        CustomerId = lruc.LoanRequest.CustomerId,
                        CustomerName = lruc.Customer.Name,
                        AssignedOfficerId = lruc.LoanRequest.AssignedOfficerId,
                        AssignedOfficerName = uo != null ? uo.Name : null,
                        LoanType = lruc.LoanRequest.LoanType,
                        Amount = lruc.LoanRequest.Amount,
                        Status = lruc.LoanRequest.Status,
                        RequestDate = lruc.LoanRequest.RequestDate,
                        Details = lruc.LoanRequest.Details
                    })
                .ToListAsync();

            return Ok(loanRequests);
        }

        // GET: api/loanrequests/customer/2
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<IEnumerable<LoanRequestViewModel>>> GetCustomerLoanRequests(int customerId)
        {
            // TODO: Verify customerId matches authenticated user

            var loanRequests = await _context.LoanRequests
                .Where(lr => lr.CustomerId == customerId)
                .Join(_context.Users,
                    lr => lr.CustomerId,
                    uc => uc.UserId,
                    (lr, uc) => new { LoanRequest = lr, Customer = uc })
                .GroupJoin(_context.Users,
                    lruc => lruc.LoanRequest.AssignedOfficerId,
                    uo => uo.UserId,
                    (lruc, uo) => new { lruc.LoanRequest, lruc.Customer, Officers = uo })
                .SelectMany(
                    lruc => lruc.Officers.DefaultIfEmpty(),
                    (lruc, uo) => new LoanRequestViewModel
                    {
                        RequestId = lruc.LoanRequest.RequestId,
                        CustomerId = lruc.LoanRequest.CustomerId,
                        CustomerName = lruc.Customer.Name,
                        AssignedOfficerId = lruc.LoanRequest.AssignedOfficerId,
                        AssignedOfficerName = uo != null ? uo.Name : null,
                        LoanType = lruc.LoanRequest.LoanType,
                        Amount = lruc.LoanRequest.Amount,
                        Status = lruc.LoanRequest.Status,
                        RequestDate = lruc.LoanRequest.RequestDate,
                        Details = lruc.LoanRequest.Details
                    })
                .ToListAsync();

            return Ok(loanRequests);
        }

        // GET: api/loanrequests/officer/3
        [HttpGet("officer/{officerId}")]
        [Authorize(Roles = "Officer")]
        public async Task<ActionResult<IEnumerable<LoanRequestViewModel>>> GetOfficerLoanRequests(int officerId)
        {
            // TODO: Verify officerId matches authenticated user

            var loanRequests = await _context.LoanRequests
                .Where(lr => lr.AssignedOfficerId == officerId)
                .Join(_context.Users,
                    lr => lr.CustomerId,
                    uc => uc.UserId,
                    (lr, uc) => new { LoanRequest = lr, Customer = uc })
                .GroupJoin(_context.Users,
                    lruc => lruc.LoanRequest.AssignedOfficerId,
                    uo => uo.UserId,
                    (lruc, uo) => new { lruc.LoanRequest, lruc.Customer, Officers = uo })
                .SelectMany(
                    lruc => lruc.Officers.DefaultIfEmpty(),
                    (lruc, uo) => new LoanRequestViewModel
                    {
                        RequestId = lruc.LoanRequest.RequestId,
                        CustomerId = lruc.LoanRequest.CustomerId,
                        CustomerName = lruc.Customer.Name,
                        AssignedOfficerId = lruc.LoanRequest.AssignedOfficerId,
                        AssignedOfficerName = uo != null ? uo.Name : null,
                        LoanType = lruc.LoanRequest.LoanType,
                        Amount = lruc.LoanRequest.Amount,
                        Status = lruc.LoanRequest.Status,
                        RequestDate = lruc.LoanRequest.RequestDate,
                        Details = lruc.LoanRequest.Details
                    })
                .ToListAsync();

            return Ok(loanRequests);
        }

        // POST: api/loanrequests
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<ActionResult<LoanRequestViewModel>> CreateLoanRequest([FromBody] LoanRequestCreateViewModel model)
        {
            // TODO: Verify model.CustomerId matches authenticated user

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loanRequest = new LoanRequest
            {
                CustomerId = model.CustomerId,
                AssignedOfficerId = model.AssignedOfficerId,
                LoanType = model.LoanType,
                Amount = model.Amount,
                Status = "pending",
                RequestDate = DateTime.Now,
                Details = model.Details
            };

            _context.LoanRequests.Add(loanRequest);
            await _context.SaveChangesAsync();

            var result = await _context.LoanRequests
                .Where(lr => lr.RequestId == loanRequest.RequestId)
                .Join(_context.Users,
                    lr => lr.CustomerId,
                    uc => uc.UserId,
                    (lr, uc) => new { LoanRequest = lr, Customer = uc })
                .GroupJoin(_context.Users,
                    lruc => lruc.LoanRequest.AssignedOfficerId,
                    uo => uo.UserId,
                    (lruc, uo) => new { lruc.LoanRequest, lruc.Customer, Officers = uo })
                .SelectMany(
                    lruc => lruc.Officers.DefaultIfEmpty(),
                    (lruc, uo) => new LoanRequestViewModel
                    {
                        RequestId = lruc.LoanRequest.RequestId,
                        CustomerId = lruc.LoanRequest.CustomerId,
                        CustomerName = lruc.Customer.Name,
                        AssignedOfficerId = lruc.LoanRequest.AssignedOfficerId,
                        AssignedOfficerName = uo != null ? uo.Name : null,
                        LoanType = lruc.LoanRequest.LoanType,
                        Amount = lruc.LoanRequest.Amount,
                        Status = lruc.LoanRequest.Status,
                        RequestDate = lruc.LoanRequest.RequestDate,
                        Details = lruc.LoanRequest.Details
                    })
                .FirstOrDefaultAsync();

            return CreatedAtAction(nameof(GetCustomerLoanRequests), new { customerId = result.CustomerId }, result);
        }
    }
}