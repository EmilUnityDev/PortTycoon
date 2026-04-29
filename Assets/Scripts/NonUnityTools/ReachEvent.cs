public class ReachEvent
{
    public static bool IsEventReached(float first, float second, bool isFirstNeedReachSecond)
    {
        if (isFirstNeedReachSecond)
        {
            if (first > second)
            {
                return true;
            }
        }
        else
        {
            if (first < second)
            {
                return true;
            }
        }

        return false;
    }
}