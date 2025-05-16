using ExitGames.Client.Photon;

public class RoomPropertiesHandler
{
   
    public static string TIME_PROP_KEY = "t";

    public Hashtable Hashtable { get; private set; }

   
    private int _time = 300;
    public int Time
    {
        get { return (int)Hashtable[TIME_PROP_KEY]; }
        set
        {
            _time = value;
            Hashtable[TIME_PROP_KEY] = _time;
        }
    }
  
    public RoomPropertiesHandler()
    {
        Hashtable = new Hashtable()
        {
            { TIME_PROP_KEY, _time },
        };
    }

    public RoomPropertiesHandler(Hashtable customPropertiesTable)
    {
        Hashtable = customPropertiesTable;
    }


}
