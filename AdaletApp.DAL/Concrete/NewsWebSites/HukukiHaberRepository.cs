using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Utilites;
using AdaletApp.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdaletApp.DAL.Concrete
{
    public class HukukiHaberRepository : IHukukiHaberRepository
    {


        private readonly IArticleRepository _articleRepository;
        public HukukiHaberRepository(IArticleRepository _articleRepository)
        {
            this._articleRepository = _articleRepository;
        }
        public async Task ArticleSourceList(string categorySourceUrl, int CategoryID)
        {
            using (var client = new HttpClient())
            {
                var document = await client.GetAsync(categorySourceUrl);
                if (!document.IsSuccessStatusCode)
                    return;

                var doc = new HtmlDocument();
                doc.LoadHtml(await document.Content.ReadAsStringAsync());

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='yatayhaberler']//div[@class='span4']//a");
                nodes.Reverse();
                foreach (var node in nodes)
                {
                    await AddArticleToDb(node.Attributes["href"]?.Value, CategoryID);
                }

            }
        }
        public async Task AddArticleToDb(string articleSourceUrl, int CategoryID)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                using (var client = new HttpClient(clientHandler))
                {
                    var document = await client.GetAsync(articleSourceUrl);
                    if (!document.IsSuccessStatusCode)
                        return;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(await document.Content.ReadAsStringAsync());

                    HtmlNode nodeTitle = doc.DocumentNode.SelectSingleNode("//h1[@itemprop='name']");
                    if (nodeTitle != null)
                    {
                        var title = HttpUtility.HtmlDecode(nodeTitle.InnerText);
                        if (await _articleRepository.HasArticle(title) == true)
                        {
                            var article = new Article();
                            article.Title = title;

                            HtmlNode subDesc = doc.DocumentNode.SelectSingleNode("//h2[@itemprop='description']");
                            if (subDesc != null)
                            {
                                article.SubTitle = HttpUtility.HtmlDecode(subDesc.InnerText);
                            }
                            HtmlNode nodeContent = doc.DocumentNode.SelectSingleNode("//div[@itemprop='articleBody']");
                            if (nodeContent != null)
                            {
                                article.NewsContent = HttpUtility.HtmlDecode(nodeContent.InnerHtml);
                            }

                            HtmlNode pictureNode = doc.DocumentNode.SelectSingleNode("//div[@class='clearfix newspic']//span//img");
                            if (pictureNode != null)
                            {

                                var picUrl = pictureNode.Attributes["src"]?.Value;
                                var fileExt = Path.GetExtension(picUrl);
                                var fileName = Helper.KarakterDuzelt(title) + fileExt;
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", fileName);
                                var imageBytes = await client.GetByteArrayAsync(picUrl);
                                await File.WriteAllBytesAsync(filePath, imageBytes);
                                article.PictureUrl = fileName;


                            }

                            article.SeoUrl = Helper.KarakterDuzelt(title);
                            article.CategoryId = CategoryID;
                            article.SourceUrl = articleSourceUrl;
                            article.Source = SourceList.HUKUKHABERLERI;
                            article.Active = true;

                            await this._articleRepository.Add(article);
                        }
                    }

                }
            }

            catch (Exception ex)
            {

            }
        }
    }
}
