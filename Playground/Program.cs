string text = "this is my message";

var result = Validate(text);
Console.WriteLine($"IsValid: {result.isValid}, message: {result.message}");

(string msg, bool valid) = Validate(text);
Console.WriteLine($"IsValid: {valid}, message: {msg}");

(string message, _) = Validate(text);
Console.WriteLine($"message: {message}");

(string message, bool isValid) Validate(string message)
{
    if (message == "this is my message")
    {
        return (message, true);
    } else
    {
        return (message, false);
    }
}
