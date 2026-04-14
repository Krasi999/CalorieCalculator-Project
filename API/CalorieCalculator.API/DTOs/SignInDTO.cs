public class SignInRequest
{
    public string Email { get; set; }

    public string Password { get; set; }
}

public class SignInResponse
{
    public bool Success { get; set; }
}