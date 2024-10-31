using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using JetBrains.Annotations;
using UnityEditor.PackageManager;

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
        public class GameServiceResponseData
        {
            public int gameId;
            public string adminUserId;
            public int playMoney;
        }

        [Serializable]
        public class GameServiceResponse
        {
            public bool success;
            public GameServiceResponseData data;
            public string error; // errorがnullであっても、文字列型で扱う
        }

        public async Task<GameServiceResponse> GetGameService()
        {
            var response = await client.GetAsync(apiUrl+"/service/getGameService");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<GameServiceResponse>(json);
        }

        [Serializable]
        public class GetUserInfoeResponseData
        {
            public int uuid;
            public string userName;

            public string userInfo;
        }

        [Serializable]
        public class GetUserInfoResponse
        {
            public bool success;
            public GetUserInfoeResponseData data;
            public string error; // errorがnullであっても、文字列型で扱う
        }

        [Serializable]
        public class GetUserInfoRequestData
        {
            public string uuid;
        }


        public async Task<GetUserInfoResponse> GetUserInfo(GetUserInfoRequestData data)
        {
            
            string jsonData = JsonUtility.ToJson(data);
            StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl+"/service/getUserInfo",content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<GetUserInfoResponse>(json);
        }



        [Serializable]
        public class SetUserInfoResponseData
        {
            public int uuid;
            public string userName;

            public string userInfo;
        }

        [Serializable]
        public class SetUserInfoResponse
        {
            public bool success;
            public SetUserInfoResponseData data;
            public string error; // errorがnullであっても、文字列型で扱う
        }

        [Serializable]
        public class SetUserInfoRequestData
        {
            public string uuid;
            public string data;
        }

        public async Task<SetUserInfoResponse> SetUserInfo(SetUserInfoRequestData data)
        {
            
            string jsonData = JsonUtility.ToJson(data);
            StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl+"/service/setUserInfo",content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            return JsonUtility.FromJson<SetUserInfoResponse>(json);
        }



        [Serializable]
        public class AddUserPointResponseData
        {
            public string uuid;

            public int money;
        }

        [Serializable]
        public class AddUserPointResponse
        {
            public bool success;
            public AddUserPointResponseData data;
            public string error; // errorがnullであっても、文字列型で扱う
        }

        [Serializable]
        public class AddUserPointRequestData
        {
            public string uuid;
            public int point;
        }
        public async Task<AddUserPointResponse> AddUserPoint(AddUserPointRequestData data)
        {
            
            string jsonData = JsonUtility.ToJson(data);
            StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl+"/service/addUserPoint",content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<AddUserPointResponse>(json);
        }



        [Serializable]
        public class GetCodeResponseData
        {
            public string loginCode;
        }

        [Serializable]
        public class GetCodeResponse
        {
            public bool success;
            public GetCodeResponseData data;
            public string error; // errorがnullであっても、文字列型で扱う
        }

        public async Task<GetCodeResponse> GetCode()
        {
            var response = await client.GetAsync(apiUrl+"/service/getCode");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonUtility.FromJson<GetCodeResponse>(json);
        }


        [Serializable]
        public class GetResultCodeRequestData
        {
            public string code;
        }
        
        [Serializable]
        public class GetResultCodeResponseData{
            [CanBeNull] public string resultUUID;
        }

        [Serializable]
        public class GetResultCodeResponse{
            public bool success;
            public GetResultCodeResponseData data;
            public string error; 
        }
        
        public async Task<GetResultCodeResponse> GetResultCode(GetResultCodeRequestData data)
        {
            string jsonData = JsonUtility.ToJson(data);
            StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl+"/service/getResultCode",content);
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
            await LoadImage($"https://api.qrserver.com/v1/create-qr-code/?data={codeData.data.loginCode}&size=200x200", renderer);
            
            bool isLogin = false;
            string uuid = "";
            
            while (!isLogin)
            {
                try
                {
                    GetResultCodeRequestData resultData = new GetResultCodeRequestData
                    {
                        code = codeData.data.loginCode
                    };

                    GetResultCodeResponse result = await GetResultCode(resultData);

                    if (result.data.resultUUID != "")
                    {
                        Debug.Log(result.data.resultUUID);
                        uuid = result.data.resultUUID;
                        isLogin = true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
                
                await Task.Delay(5000);
            }
            
            bool isFinish = false;

            while (!isFinish)
            {
                if (uuid != "")
                {
                   AddUserPointRequestData addData = new AddUserPointRequestData{
                       uuid = uuid,
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
