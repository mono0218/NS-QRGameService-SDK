using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using JetBrains.Annotations;

namespace NSQRGameService
{
    public class GameService: MonoBehaviour
    {
        private string apiUrl;
        private string apiKey;
        private HttpClient client;
        public void Initialize(string url, string key)
        {
            client = new HttpClient();
            apiUrl = url;
            apiKey = key;
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
        }
        
        [Serializable]
        public class AddUserPointRequestData
        {
            public string userId;
            public int point;
        }
        public async Task AddUserPoint(AddUserPointRequestData data)
        {
            try
            {
                string jsonData = JsonUtility.ToJson(data);
                StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl+"/v1/service/points",content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
        
        [Serializable]
        public class GetCodeResponseData
        {
            public string code;
        }

        [Serializable]
        public class GetCodeResponse
        {
            public bool success;
            public GetCodeResponseData data;
            public string message; // errorがnullであっても、文字列型で扱う
        }

        public async Task<GetCodeResponse> GetCode()
        {
            var response = await client.GetAsync(apiUrl+"/v1/qrcode");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<GetCodeResponse>(json);
        }
        
        [Serializable]
        public class GetResultCodeResponseData
        {
            public bool isDone;
            public string id;
            public string username;
            public int point;
        }

        [Serializable]
        public class GetResultCodeResponse{
            public bool success;
            public GetResultCodeResponseData data;
            public string message; 
        }
        
        public async Task<GetResultCodeResponse> GetResultCode(string qrcode)
        {
            var response = await client.GetAsync(apiUrl+"/v1/qrcode/"+qrcode);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<GetResultCodeResponse>(json);
        }
        
        public async Task LoadImage(string url, Renderer renderer)
        {
            try
            {
                byte[] imageData = await client.GetByteArrayAsync(url);
                Texture2D texture = new Texture2D(2, 2); // 初期サイズは適当で大丈夫
                texture.LoadImage(imageData); // 画像データをテクスチャに読み込み
                
                if (renderer != null)
                {
                    // マテリアルのメインテクスチャにテクスチャを設定
                    renderer.material.mainTexture = texture;
                }
                else
                {
                    Debug.LogError("Rendererコンポーネントが見つかりません。");
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"HTTPリクエストエラー: {e.Message}");
            }
        }

        public async Task<string> AllProcess(int point, Renderer renderer)
        {
            GetCodeResponse codeData = await GetCode();
            await LoadImage($"https://api.qrserver.com/v1/create-qr-code/?data={codeData.data.code}&size=200x200", renderer);
            
            bool isLogin = false;
            string uuid = "";
            
            while (!isLogin)
            {
                try
                {
                    GetResultCodeResponse result = await GetResultCode(codeData.data.code);
                    Debug.Log(result.data.isDone);

                    if (result.data.isDone)
                    {
                        uuid = result.data.id;
                        isLogin = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                
                await Task.Delay(5000);
            }
            
            bool isFinish = false;

            while (!isFinish)
            {
                if (uuid != "")
                {
                   AddUserPointRequestData addData = new AddUserPointRequestData{
                       userId = uuid,
                       point = point
                   };
                   
                   await AddUserPoint(addData); 
                   Debug.Log(addData);
                   isFinish = true;
                }
                await Task.Delay(5000);
            }
            
            return "success";
        }
    }
}

