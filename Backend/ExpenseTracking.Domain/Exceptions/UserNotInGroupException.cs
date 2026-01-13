namespace ExpenseTracking.Domain.Exceptions
{
    public class UserNotInGroupException : BaseException
    {
        public int UserId { get; private set; }
        public int GroupId { get; private set; }
        
        public UserNotInGroupException(int userId, int groupId) 
            : base($"User {userId} is not a member of group {groupId}", "USER_NOT_IN_GROUP")
        {
            UserId = userId;
            GroupId = groupId;
        }
    }
}