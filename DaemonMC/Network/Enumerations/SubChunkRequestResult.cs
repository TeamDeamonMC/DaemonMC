namespace DaemonMC.Network.Enumerations
{
    public enum SubChunkRequestResult : byte
    {
        Undefined = 0,
        Success = 1,
        LevelChunkDoesntExist = 2,
        WrongDimension = 3,
        PlayerDoesntExist = 4,
        IndexOutOfBounds = 5,
        SuccessAllAir = 6
    }

    public enum HeightMapDataType : byte
    {
        NoData = 0,
        HasData = 1,
        AllTooHigh = 2,
        AllTooLow = 3,
        AllCopied = 4
    }
}
