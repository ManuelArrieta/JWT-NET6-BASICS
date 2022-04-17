namespace JWT_BASICS.Models
{
    public class ResponseAuthentication
    {
        public string Token { get; set; }
        public int HttpStatusCode { get; set; }
        public string HttpStatusTitle { get; set; }
    }
}
