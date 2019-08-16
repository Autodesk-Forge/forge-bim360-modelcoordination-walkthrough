namespace MCSample.Model
{
    public enum IndexFieldType : int
    {
        Unknown = 0,
        Boolean = 1,
        Integer = 2,
        Double = 3,
        BLOB = 10,
        DbKey = 11,
        String = 20,
        LocalisableString = 21,
        DateTime = 22,
        GeoLocation = 23,
        Position = 24
    }
}
