namespace HurtJuice;

public static class StringReplacer
{
    // List of possible 8-character replacement strings
    private static readonly string[] ReplacementList = new string[]
        {"abcdefgh", "ijklmnop", "qrstuvwx", "yzabcdef", "ghijklmn"};

    private static readonly Random Random = new Random();

    public static string ReplaceMiddleRandomly(string originalString)
    {
        // Check if we should replace
        if (Random.NextDouble() < 0.5)
        {
            Console.WriteLine("********REPLACED STRING********");
            // Calculate the start index for replacement to ensure it's in the middle
            int startIdx = originalString.Length / 2 - 4;
            // Choose a random replacement string
            string replacement = ReplacementList[Random.Next(ReplacementList.Length)];
            // Replace the middle part of the string
            string newString = originalString.Substring(0, startIdx) + replacement +
                               originalString.Substring(startIdx + 8);
            return newString;
        }
        else
        {
            // Return the original string if no replacement is to be made
            return originalString;
        }
    }

    public static bool HasReplacementInString(string checkString)
    {
        // Check if any of the replacement strings are in the checkString
        foreach (string replacement in ReplacementList)
        {
            if (checkString.Contains(replacement))
            {
                return true;
            }
        }

        return false;
    }
}