using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Abstract.NewsWebSites;
using AdaletApp.DAL.Utilites;
using AdaletApp.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdaletApp.DAL.Concrete.NewsWebSites
{
    public class AdaletMedyaRepository : IAdaletMedyaRepository
    {
        private readonly IArticleRepository _articleRepository;
        public AdaletMedyaRepository(IArticleRepository _articleRepository)
        {
            this._articleRepository = _articleRepository;
        }

        public async Task ArticleSourceList(string categorySourceUrl, int CategoryID)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var client = new HttpClient(clientHandler))
                {
                    var document = await client.GetAsync(categorySourceUrl);
                    if (!document.IsSuccessStatusCode)
                        return;

                    var doc = new HtmlDocument();
                    doc.LoadHtml(await document.Content.ReadAsStringAsync());

                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//article[@class='tek_genis_fotolu_by_category1']//a");
                    var list = nodes.Reverse();

                    List<Task> taskList = new List<Task>();
                    foreach (var node in list)
                    {
                        taskList.Add(AddArticleToDb(node.Attributes["href"]?.Value, CategoryID));
                    }
                    Task.WaitAll(taskList.ToArray());

                }
            }
            catch (Exception)
            {
                return;
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

                    HtmlNode nodeTitle = doc.DocumentNode.SelectSingleNode("//h1[@class='hbaslik pdlr-20']");

                    if (nodeTitle == null)
                    {
                        nodeTitle = doc.DocumentNode.SelectSingleNode("//div[@class='yazarin_yazi_basligi']");
                    }
                    if (nodeTitle == null)
                        return;
                    var title = HttpUtility.HtmlDecode(nodeTitle.InnerText);
                    title = !string.IsNullOrEmpty(title) ? title.Trim() : title;
                    if (await _articleRepository.HasArticle(title) == true)
                    {
                        var article = new Article();
                        article.Title = title;

                        //HtmlNode subDesc = doc.DocumentNode.SelectSingleNode("//p[@class='lead hs-head-font']");
                        //if (subDesc != null)
                        //{
                        //    article.SubTitle = HttpUtility.HtmlDecode(subDesc.InnerText);
                        //}
                        article.SubTitle = title;
                        HtmlNode nodeContent = doc.DocumentNode.SelectSingleNode("//div[@class='icerik_detay']");
                        if (nodeContent != null)
                        {
                            HtmlNodeCollection reklamNode = nodeContent.SelectNodes("//div[@class='reklam']");
                            if (reklamNode != null)
                            {
                                reklamNode.ToList().ForEach(node =>
                                {
                                    node.Remove();
                                });
                            }
                            article.NewsContent = HttpUtility.HtmlDecode(nodeContent.InnerHtml);
                        }

                        HtmlNode pictureNode = doc.DocumentNode.SelectSingleNode("//div[@class='onecikan_gorsel']//img");
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
                        article.Source = SourceList.ADALETMEDYA;
                        article.Active = true;
                        await this._articleRepository.Add(article);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
           

        }
    }
}
