﻿using AdaletApp.DAL.Abstract;
using AdaletApp.DAL.Utilites;
using AdaletApp.Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AdaletApp.DAL.Concrete
{
    public class AdaletBizRepository : IAdaletBizRepository
    {
        private readonly IArticleRepository _articleRepository;
        public AdaletBizRepository(IArticleRepository _articleRepository)
        {
            this._articleRepository = _articleRepository;
        }
        public async Task ArticleSourceList(string categorySourceUrl, int CategoryID)
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

                HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='span hs-item hs-beh hs-kill-ml clearfix']//a");
                var list = nodes.Reverse();
                foreach (var node in list)
                {
                    await AddArticleToDb(node.Attributes["href"]?.Value, CategoryID);
                }

            }
        }

        public async Task AddArticleToDb(string articleSourceUrl, int CategoryID)
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

                HtmlNode nodeTitle = doc.DocumentNode.SelectSingleNode("//h1[@class='title hs-share-title hs-title-font-2']");
                if (nodeTitle != null)
                {
                    var title = HttpUtility.HtmlDecode(nodeTitle.InnerText);
                    if (await _articleRepository.HasArticle(title) == true)
                    {
                        var article = new Article();
                        article.Title = title;

                        HtmlNode subDesc = doc.DocumentNode.SelectSingleNode("//p[@class='lead hs-head-font']");
                        if (subDesc != null)
                        {
                            article.SubTitle = HttpUtility.HtmlDecode(subDesc.InnerText);
                        }
                        HtmlNode nodeContent = doc.DocumentNode.SelectSingleNode("//div[@id='newsbody']");
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
                        article.Source = SourceList.ADALETBIZ;
                        article.Active = true;
                        await this._articleRepository.Add(article);
                    }

                }
            }
        }
    }
}
