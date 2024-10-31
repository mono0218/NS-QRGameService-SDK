using UnityEngine;
using NSQRGameService;

public class Sample : MonoBehaviour
{
    public string apiUrl;
    public string apiKey;
    public int addPointNumber;
    private GameService gameService;
    
    async void Start()
    {
        GameObject gobject = GetComponent<GameObject>();
        gobject.SetActive(false);
        gameService = new();
        gameService.Initialize(apiUrl, apiKey);
    }

    async void addPoint()
    {
        GameObject gobject = GetComponent<GameObject>();
        
        gobject.SetActive(true);
        Renderer renderer = GetComponent<Renderer>();
        var response = await gameService.AllProcess(addPointNumber,renderer);
        
        Debug.Log(response);
        gobject.SetActive(false);
    }
}
