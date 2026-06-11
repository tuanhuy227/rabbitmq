namespace SendMailApi.DTOs
{
    public class EmailMessageDto
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}
