﻿using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Utilites;
using AdaletApp.Entities;
using HtmlAgilityPack;
using System.Web;

namespace AdaletApp.DAL.Concrete
{
    public class AdaletBizRepository : IAdaletBizRepository
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AdaletBizRepository(IArticleRepository _articleRepository, ICategoryRepository _categoryRepository)
        {
            this._articleRepository = _articleRepository;
            this._categoryRepository = _categoryRepository;

        }
        public async Task ArticleSourceList(string categorySourceUrl, int CategoryID)
        {
            var doc = new HtmlDocument();
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var client = new HttpClient(clientHandler))
                {
                    var document = await client.GetAsync(categorySourceUrl);
                    if (!document.IsSuccessStatusCode)
                    {
                        return;
                    }

                    doc.LoadHtml(await document.Content.ReadAsStringAsync());

                }
            }
            catch (Exception)
            {
                return;
            }

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='span hs-item hs-beh hs-kill-ml clearfix']//a");
            if (nodes != null)
            {
                var list = nodes.Reverse();
                List<Task> taskList = new List<Task>();
                foreach (var node in list)
                {
                    taskList.Add(AddArticleToDb(node.Attributes["href"]?.Value, CategoryID));
                }
                Task.WaitAll(taskList.ToArray());
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

                    HtmlNode nodeTitle = doc.DocumentNode.SelectSingleNode("//h1[@class='title hs-share-title hs-title-font-2']");
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

                        HtmlNode subDesc = doc.DocumentNode.SelectSingleNode("//p[@class='lead hs-head-font']");
                        if (subDesc != null)
                        {
                            article.SubTitle = HttpUtility.HtmlDecode(subDesc.InnerText);
                        }
                        else
                        {
                            article.SubTitle = title;
                        }
                        HtmlNode nodeContent = doc.DocumentNode.SelectSingleNode("//div[@id='newsbody']");
                        if (nodeContent != null)
                        {
                            article.NewsContent = HttpUtility.HtmlDecode(nodeContent.InnerHtml);
                        }

                        //HtmlNode pictureNode = doc.DocumentNode.SelectSingleNode("//div[@class='clearfix newspic']//span//img");
                        //if (pictureNode != null)
                        //{

                        //    var picUrl = pictureNode.Attributes["src"]?.Value;
                        //    var fileExt = Path.GetExtension(picUrl);
                        //    var fileName = seoTitle + fileExt;
                        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images", fileName);
                        //    var imageBytes = await client.GetByteArrayAsync(picUrl);
                        //    await File.WriteAllBytesAsync(filePath, imageBytes);
                        //    article.PictureUrl = fileName;

                        //}
                        article.SeoUrl = categoryEntity.SeoUrl + "/" + seoTitle;
                        article.CategoryId = CategoryID;
                        article.SourceUrl = articleSourceUrl;
                        article.Source = SourceList.ADALETBIZ;
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
