using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracking.Business.Services;
using ExpenseTracking.Business.DTOs;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
        }
        
        /// <summary>
        /// Create a new expense group
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDTO dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid group data");
            
            var userId = GetCurrentUserId();
            dto.OwnerId = userId;
            
            var group = await _groupService.CreateGroupAsync(dto);
            
            return Ok(new
            {
                success = true,
                message = "Group created successfully",
                group = new
                {
                    group.GroupId,
                    group.GroupName,
                    group.Description,
                    group.RequireOwnerApprovalForOverBudget
                }
            });
        }
        
        /// <summary>
        /// Add a member to the group
        /// </summary>
        [HttpPost("{groupId}/members")]
        public async Task<IActionResult> AddMember(int groupId, [FromBody] AddMemberRequestDTO request)
        {
            await _groupService.AddMemberAsync(groupId, request.UserId, request.Role);
            
            return Ok(new
            {
                success = true,
                message = "Member added successfully"
            });
        }
        
        /// <summary>
        /// Remove a member from the group
        /// </summary>
        [HttpDelete("{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveMember(int groupId, int userId)
        {
            await _groupService.RemoveMemberAsync(groupId, userId);
            
            return Ok(new
            {
                success = true,
                message = "Member removed successfully"
            });
        }
        
        /// <summary>
        /// Get all groups for current user
        /// </summary>
        [HttpGet("my-groups")]
        public async Task<IActionResult> GetMyGroups()
        {
            var userId = GetCurrentUserId();
            var groups = await _groupService.GetUserGroupsAsync(userId);
            
            return Ok(groups.Select(g => new
            {
                g.GroupId,
                g.GroupName,
                g.Description,
                g.IsActive,
                IsOwner = g.OwnerId == userId
            }));
        }
        
        /// <summary>
        /// Get group details with members
        /// </summary>
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetGroupDetails(int groupId)
        {
            var group = await _groupService.GetGroupDetailsAsync(groupId);
            
            return Ok(group);
        }
        
        /// <summary>
        /// Update group settings
        /// </summary>
        [HttpPut("{groupId}")]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] UpdateGroupDTO dto)
        {
            var group = await _groupService.UpdateGroupAsync(groupId, dto);
            
            return Ok(new
            {
                success = true,
                message = "Group updated successfully",
                group
            });
        }
        
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException("User ID not found in token");
            
            return int.Parse(userIdClaim);
        }
    }
}