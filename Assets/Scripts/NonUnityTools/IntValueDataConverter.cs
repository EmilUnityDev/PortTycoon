public class IntValueDataConverter
{
    private static string[] _literas = new string[]
    {
        "k", "m", "b", "t", "q"
    };

    public static string Convert(int startValue)
    {
        if (startValue < 1000)
        {
            return startValue.ToString();
        }

        int multiplicity = 1000;

        int indexes = 0;

        while (startValue / (multiplicity * 1000) > 0)
        {
            multiplicity *= 1000;
            indexes++;
        }

        string returnString = (startValue / multiplicity).ToString();

        startValue = startValue % multiplicity;

        if (startValue != 0)
        {
            returnString += "." + (startValue / (multiplicity / 10)).ToString();
        }

        returnString += _literas[indexes];

        return returnString;
    }
}

public static class Convertation
{
    public static string ConvertToString(this int value)
    {
        return IntValueDataConverter.Convert(value);
    }
}