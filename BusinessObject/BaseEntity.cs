namespace BusinessObject
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool IsDelete { get; set; } = false;
    }
}
