namespace MUSM_api_MVCwebapp.Dtos
{
    public class EditTaskDto
    {
        public int Id { get; set; }
      
        public string? Title { get; set; }

        
        public string? Description { get; set; }

       
        public string? Location { get; set; }

        public string? CompletionStatus { get; set; } = "Open";

        
        public string? Priority { get; set; }

        
        public string? Category { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? DateCompleted { get; set; }

       
        public string? WorkerId { get; set; }
       
        public int? RequestId { get; set; }

       
      
    }
}
