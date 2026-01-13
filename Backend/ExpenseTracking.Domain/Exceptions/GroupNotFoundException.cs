namespace ExpenseTracking.Domain.Exceptions
{
    public class GroupNotFoundException : EntityNotFoundException
    {
        public GroupNotFoundException(int groupId) 
            : base("ExpenseGroup", groupId)
        {
        }
    }
}