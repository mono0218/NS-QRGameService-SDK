using UnityEngine;
using NSQRGameService;

public class Sample : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        GameService gameService = new("http://localhost:3000", "*****");
    
        var response = await gameService.GetGameService();
        Debug.Log(response.data.gameId);
    }
}
