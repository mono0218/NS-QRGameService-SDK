using UnityEngine;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Unity.Plastic.Newtonsoft.Json;

namespace NSQRGameService
{
    public class GameService: MonoBehaviour
    {
        private string apiUrl;
        private string apiKey;
        private HttpClient client;

        public GameService(string url, string key)
        {
            client = new HttpClient();
            apiUrl = url;
            apiKey = key;
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
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
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
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
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
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
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
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
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
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
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
        public class GetResultCodeResponse{

            public bool success;
            public string error; 
        }

        [Serializable]
        public class GetResultCodeResponseData
        {
            public GetResultCodeResponseData data;
        }

        [Serializable]
        public class GetResultCodeSuccessResponse : GetResultCodeResponse
        {
            public GetResultCodeResponseData data;
        }

        public async Task<GetResultCodeResponse> GetResultCode(GetResultCodeRequestData data)
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            string jsonData = JsonUtility.ToJson(data);
            StringContent content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl+"/service/getResultCode",content);
            if (response.StatusCode.ToString() == "404")
            {
                GetResultCodeResponse json = new GetResultCodeResponse{
                    success = false,
                    error = "404"
                };
                return json;
            }else{
                var json = await response.Content.ReadAsStringAsync();
                return JsonUtility.FromJson<GetResultCodeSuccessResponse>(json);
            }
        }
    }
}
