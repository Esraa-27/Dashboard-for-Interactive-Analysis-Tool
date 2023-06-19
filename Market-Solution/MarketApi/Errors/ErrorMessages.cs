namespace MarketApi.Errors
{
    public static class ErrorMessages
    {
        public static string Error
          => "An Error occured";

       public static string FileEmpty
         => "File is empty.";

        public static string FileNotValid
         => "File is not Valid.";
        public static string DataSuccess
            =>"Data successfully imported.";
        public static string DataNotSuccess
           => "can't read file .";

        public static string LoginFiled
            => "The email and password do not match.";
        public static string InvalidToken
            => "Invalid Token";
    }
}
