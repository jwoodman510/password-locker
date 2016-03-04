namespace Ksu.Global.Constants
{
    public static class Keys
    {
        public static string Biscuits = "$4LTie81$cU1T$";

        public static string SaltyBiscuits(string salt)
        {
            return $"{salt}{Biscuits}";
        }
    }
}
