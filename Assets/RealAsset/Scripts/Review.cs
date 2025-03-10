using System;

[Serializable]
public class Review
{
    public string Username;
    public string Text;
    public string Timestamp;

  

    public Review(string username, string text, string timestamp)
    {
        Username = username;
        Text = text;
        Timestamp = timestamp;
    }
}