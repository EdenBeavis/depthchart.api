namespace depthchart.api.Infrastructure
{
    public static class Constants
    {
        public const int MinimumPositionDepth = 0;
        public const int AddtionalPositionDepth = 1;

        public static class ErrorMessages
        {
            public const string PlayerDoesNotExist = "Player does not exist.";
            public const string PlayerNumber = "Player number must be equal to or greater than 0 and equal to or less than 99.";
            public const string PlayerName = "Player name must have a value and be less than 256 characters.";

            public const string DepthPostionDepth = "Depth Position depth must be equal to or greater than 0 and equal to or less than 99.";
            public const string DepthPostionPosition = "Depth Postiion position must have a value and be less than 256 characters.";
        }
    }
}