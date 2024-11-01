using System.Threading.Tasks;
using UnityEngine;
using NSQRGameService;

public class Sample : MonoBehaviour
{
    public string apiUrl;
    public string apiKey;
    public int addPointNumber;
    private GameService gameService;
    public GameObject gobject;
    
    async void Start()
    {
        gobject.SetActive(false);
        gameService = new();
        gameService.Initialize(apiUrl, apiKey);
        
        await addPoint();
    }

    async Task addPoint()
    {
        gobject.SetActive(true);
        Renderer renderer = GetComponent<Renderer>();
        var response = await gameService.AllProcess(addPointNumber,renderer);
        
        Debug.Log(response);
        gobject.SetActive(false);
    }
}
