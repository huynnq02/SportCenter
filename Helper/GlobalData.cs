namespace SportCenter.Helper
{
    public static class GlobalData
    {
        public static string username { get; set; }
        public static string name { get; set; }
        public static bool isLoggedIn { get; set; }
        public static string role { get; set; }
        public static int id { get; set; }
        public static void ClearData()
        {
            username = string.Empty;
            isLoggedIn = false;
            role = string.Empty;
            id = 0;
        }
    }
}
