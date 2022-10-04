namespace Hotel.Indexes
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ComputeStatus
    {
        public static string Do(IEnumerable<string> statuses)
        {
            if (statuses.Contains("NotInUse"))
                return "NotInUse";

            if (statuses.Contains("GuestsIn"))
                return "GuestsIn";

            if (statuses.Contains("ReadyForGuests"))
                return "ReadyForGuests";

            if (statuses.Contains("Available"))
                return "Available";

            return "xxx";
        }
    }
}
