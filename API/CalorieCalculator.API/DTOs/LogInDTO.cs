public class LogInRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LogInResponse
{
    public bool Success { get; set; }
}