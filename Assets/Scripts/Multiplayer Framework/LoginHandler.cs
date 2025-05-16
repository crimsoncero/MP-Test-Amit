using Photon.Pun;

public class LoginHandler : MonoBehaviourPunCallbacks
{
    private ConnectionManager Con => ConnectionManager.Instance;

    public void Connect()
    {
        Con.Connect();
    }

    public override void OnConnectedToMaster()
    {
        // Turn off the login panel after connecting
        gameObject.SetActive(false);
    }
}
