namespace ExpenseTracking.Domain.Entities
{
    public abstract class BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
        protected BaseEntity()
        {
            CreatedDate = DateTime.Now;
        }
        
        public virtual void Update()
        {
            ModifiedDate = DateTime.Now;
        }
    }
}
