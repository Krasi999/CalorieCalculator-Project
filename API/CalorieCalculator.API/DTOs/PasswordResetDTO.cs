public class ForgotPasswordRequest
{
    public string Email { get; set; }
}

public class ForgotPasswordResponse
{
    public bool Success { get; set; }
    public string Code { get; set; }  // Засега връщаме кода, за да го покажем в dialog
}

public class VerifyCodeRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
}

public class ResetPasswordRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
}