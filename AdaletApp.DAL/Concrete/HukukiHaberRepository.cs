using AdaletApp.DAL.Abstract;
using AdaletApp.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaletApp.DAL.Concrete
{
    public class HukukiHaberRepository : IHukukiHaberRepository
    {


        public async Task<List<string>> ArticleSourceList(string categorySourceUrl)
        {
            using (var client = new HttpClient())
            {
                var document = await client.GetAsync("https://www.hukukihaber.net/ozel-hukuk");
                if (!document.IsSuccessStatusCode)
                    return null;

                var doc = new HtmlDocument();
                doc.LoadHtml(await document.Content.ReadAsStringAsync());

                var list = new List<string>();

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='yatayhaberler']//div[@class='span4']//a");
                foreach (var node in nodes)
                {
                    list.Add(node.Attributes["href"]?.Value);
                }
                return list;
            }
        }
    }
}
