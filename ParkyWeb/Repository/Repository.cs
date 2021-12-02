using Newtonsoft.Json;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IHttpClientFactory _clientFactory;
        public Repository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<bool> CreateAsync(string url, T objToCreate)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            if(objToCreate != null)
            {
                req.Content = new StringContent(JsonConvert.SerializeObject(objToCreate), Encoding.UTF8, "application/json");
            }
            else
            {
                return false;
            }
            var client = _clientFactory.CreateClient();
            HttpResponseMessage res = await client.SendAsync(req);
            if (res.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string url, int Id)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, url+Id);

            var client = _clientFactory.CreateClient();
            HttpResponseMessage res = await client.SendAsync(req);
            if (res.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public Task<bool> DeleteAsync(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync(string url)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _clientFactory.CreateClient();
            HttpResponseMessage res = await client.SendAsync(req);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            }
            else
            {
                return null;
            }
        }

        public async Task<T> GetAsync(string url, int Id)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, url);

            var client = _clientFactory.CreateClient();
            HttpResponseMessage res = await client.SendAsync(req);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await res.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdateAsync(string url, T objToUpdate)
        {
            var req = new HttpRequestMessage(HttpMethod.Patch, url);
            if (objToUpdate != null)
            {
                req.Content = new StringContent(JsonConvert.SerializeObject(objToUpdate), Encoding.UTF8, "application/json");
            }
            else
            {
                return false;
            }
            var client = _clientFactory.CreateClient();
            HttpResponseMessage res = await client.SendAsync(req);
            if (res.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
