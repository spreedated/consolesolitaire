namespace ConsoleSolitaire.Extensions
{
    internal static class Extensions
    {
        internal static bool IsOdd(this int integ)
        {
            return integ % 2 != 0;
        }

        internal static bool IsEven(this int integ)
        {
            return integ % 2 == 0;
        }
    }
}
