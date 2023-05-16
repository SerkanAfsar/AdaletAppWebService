using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Utilites;
using AdaletApp.Entities;
using HtmlAgilityPack;
using System.Web;

namespace AdaletApp.DAL.Concrete
{
    public class HukukiHaberRepository : IHukukiHaberRepository
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;
        public HukukiHaberRepository(IArticleRepository _articleRepository, ICategoryRepository categoryRepository)
        {
            this._articleRepository = _articleRepository;
            this._categoryRepository = categoryRepository;
        }
        public async Task ArticleSourceList(string categorySourceUrl, int CategoryID)
        {
            using (var client = new HttpClient())
            {
                var doc = new HtmlDocument();
                try
                {
                    var document = await client.GetAsync(categorySourceUrl);
                    if (!document.IsSuccessStatusCode)
                    {
                        return;
                    }

                    doc.LoadHtml(await document.Content.ReadAsStringAsync());
                }
                catch (Exception)
                {
                    return;
                }
                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='card-body p-3']//h4//a");
                if (nodes != null)
                {
                    var list = nodes.Reverse();
                    List<Task> taskList = new List<Task>();
                    foreach (var node in list)
                    {
                        var nodeUrl = node.Attributes["href"]?.Value;
                        if (nodeUrl != null)
                        {
                            taskList.Add(AddArticleToDb(nodeUrl.IndexOf("https://www.hukukihaber.net") > -1 ? nodeUrl : "https://www.hukukihaber.net" + nodeUrl, CategoryID));
                        }

                    }
                    Task.WaitAll(taskList.ToArray());
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
                    {
                        return;
                    }

                    var doc = new HtmlDocument();
                    doc.LoadHtml(await document.Content.ReadAsStringAsync());

                    HtmlNode nodeTitle = doc.DocumentNode.SelectSingleNode("//h1");
                    if (nodeTitle == null)
                    {
                        return;
                    }

                    var title = HttpUtility.HtmlDecode(nodeTitle.InnerText);
                    if (await _articleRepository.HasArticle(title) == true)
                    {
                        var article = new Article();
                        article.Title = title;

                        var categoryEntity = await _categoryRepository.Get(a => a.Id == CategoryID);


                        var seoTitle = Utils.KarakterDuzelt(title);
                        HtmlNode subDesc = doc.DocumentNode.SelectSingleNode("//h2");
                        if (subDesc != null)
                        {
                            article.SubTitle = HttpUtility.HtmlDecode(subDesc.InnerText);
                        }
                        else
                        {
                            article.SubTitle = title;
                        }
                        HtmlNode nodeContent = doc.DocumentNode.SelectSingleNode("//div[@class='article-text text-black container-padding']");
                        if (nodeContent != null)
                        {
                            HtmlNodeCollection reklamNode = nodeContent.SelectNodes("//div[@data-pagespeed='true']");
                            if (reklamNode != null)
                            {
                                reklamNode.ToList().ForEach(node =>
                                {
                                    node.Remove();
                                });
                            }
                            article.NewsContent = HttpUtility.HtmlDecode(nodeContent.InnerHtml);
                        }

                        HtmlNode pictureNode = doc.DocumentNode.SelectSingleNode("//div[@class='inner']//img");
                        if (pictureNode != null)
                        {
                            var picUrl = pictureNode.Attributes["src"]?.Value;
                            var fileExt = Path.GetExtension(picUrl);
                            var fileName = seoTitle + fileExt;
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", fileName);
                            var imageBytes = await client.GetByteArrayAsync(picUrl);
                            await File.WriteAllBytesAsync(filePath, imageBytes);
                            article.PictureUrl = fileName;

                        }

                        article.SeoUrl = categoryEntity.SeoUrl + "/" + seoTitle;
                        article.CategoryId = CategoryID;
                        article.SourceUrl = articleSourceUrl;
                        article.Source = SourceList.HUKUKİHABER;
                        article.Active = true;

                        await this._articleRepository.Add(article);

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
