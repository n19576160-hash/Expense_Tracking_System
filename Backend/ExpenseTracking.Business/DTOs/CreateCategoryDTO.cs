namespace ExpenseTracking.Business.DTOs
{
    public class CreateCategoryDTO
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public int? CreatedByUserId { get; set; }
    }
}