using UnityEngine;

public static class console
{
    public static void warn(string message)
    {
        Debug.LogWarning(message);
    }

    public static void warn(string message, string message2)
    {
        Debug.LogWarning(message + message2);
    }

    public static void warn(string message, string message2, string message3)
    {
        Debug.LogWarning(message + message2 + message3);
    }
}
