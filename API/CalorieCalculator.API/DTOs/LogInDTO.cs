public class LogInRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LogInResponse
{
    public bool Success { get; set; }
}