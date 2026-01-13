using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public interface IGroupService
    {
        Task<ExpenseGroup> CreateGroupAsync(CreateGroupDTO dto);
        Task AddMemberAsync(int groupId, int userId, string role);
        Task RemoveMemberAsync(int groupId, int userId);
        Task<IEnumerable<ExpenseGroup>> GetUserGroupsAsync(int userId);
        Task<GroupDetailsDTO> GetGroupDetailsAsync(int groupId);
        Task<ExpenseGroup> UpdateGroupAsync(int groupId, UpdateGroupDTO dto);
    }
}