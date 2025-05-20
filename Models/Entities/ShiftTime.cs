namespace ShiftScheduler.Models.Entities
{
    /// <summary>Matches the numbers stored in the DB (0=Morning, 1=Evening)</summary>
    public enum ShiftTime : int
    {
        Morning = 0,
        Evening = 1
    }
}
