namespace LaesoeVejr;

public class SftpOptions
{
    public required string Host { get; set; }
    public int Port { get; set; } = 22;
    public required string Username { get; set; }
    public required string Password { get; set; }
}
