namespace RentACarAPI.Controllers.User
{
    public class UserResponse
    {
        public string Message { get; set; }

        public bool isSuccess { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
