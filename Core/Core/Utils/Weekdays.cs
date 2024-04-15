public static class Weekdays
{
    public static readonly int Sunday    = (1 << 0);
    public static readonly int Monday    = (1 << 1);
    public static readonly int Tuesday   = (1 << 2);
    public static readonly int Wednesday = (1 << 3);
    public static readonly int Thursday  = (1 << 4);
    public static readonly int Friday    = (1 << 5);
    public static readonly int Saturday  = (1 << 6);

    private static readonly int NUM_WEEKDAYS = 7;

    public static int ToBitField(bool[] weekdays) {
        int bitfield = 0;

        for (int i = 0; i < weekdays.Length; i++) {
            if (weekdays[i]) {
                bitfield |= (1 << i);
            }
        }

        return bitfield;
    }

    public static bool[] FromBitField(int bitfield) {
        bool[] weekdays = new bool[NUM_WEEKDAYS];


        for (int i = 0; i < NUM_WEEKDAYS; i++) {
            weekdays[i] = (bitfield & (1 << i)) != 0;
        }

        return weekdays;
    }
}

