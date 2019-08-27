namespace Sitecore.Support.XA.Feature.SiteMetadata.Sitemap
{
    using Sitecore.Data.Items;
    using Sitecore.Links;
    using Sitecore.XA.Feature.SiteMetadata;
    using Sitecore.XA.Feature.SiteMetadata.Enums;
    using Sitecore.XA.Feature.SiteMetadata.Sitemap;
    using Sitecore.XA.Foundation.SitecoreExtensions.Extensions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public class SitemapGenerator : Sitecore.XA.Feature.SiteMetadata.Sitemap.SitemapGenerator
    {
        public SitemapGenerator()
        {
        }

        public SitemapGenerator(XmlWriterSettings xmlWriterSettings) : base(xmlWriterSettings)
        {
        }

        protected override StringBuilder BuildMultilanguageSitemap(IEnumerable<Item> childrenTree, SitemapLinkOptions options)
        {
            var urlOptions = GetUrlOptions();
            var sitemapLinkOptions = new SitemapLinkOptions(options.Scheme, urlOptions, options.TargetHostname);

            //Alternate URL should always have language embedded
            var alternateUrlOptions = (UrlOptions)urlOptions.Clone();
            alternateUrlOptions.LanguageEmbedding = LanguageEmbedding.Always;
            var sitemapLinkOptionsForAlternateLinks = new SitemapLinkOptions(options.Scheme, alternateUrlOptions, options.TargetHostname);

            //x-default alternate URL should never have language embedded
            var xDefaultOptions = (UrlOptions)sitemapLinkOptionsForAlternateLinks.UrlOptions.Clone();
            xDefaultOptions.LanguageEmbedding = LanguageEmbedding.Never;
            var sitemapLinkOptionsForXDefaultLink = new SitemapLinkOptions(options.Scheme, xDefaultOptions, options.TargetHostname);

            var pages = new List<XElement>();
            foreach (var item in childrenTree)
            {
                var changeFreq = item.Fields[Templates.Sitemap._Sitemap.Fields.ChangeFrequency].ToEnum<SitemapChangeFrequency>();
                if (changeFreq == SitemapChangeFrequency.DoNotInclude)
                {
                    continue;
                }
                List<XElement> alternateUrls = new List<XElement>();
                foreach (var itemLanguage in item.Languages)
                {
                    var itemAlternate = item.Database.GetItem(item.ID, itemLanguage);
                    if (itemAlternate.Versions.Count > 0)
                    {
                        sitemapLinkOptionsForAlternateLinks.UrlOptions.Language = itemLanguage;

                        var href = GetFullLink(itemAlternate, sitemapLinkOptionsForAlternateLinks);
                        var hreflang = itemLanguage.CultureInfo.Name;
                        var alternate = BuildAlternateLinkElement(href, hreflang);
                        alternateUrls.Add(alternate);
                    }
                }
                if (alternateUrls.Count >= 2)
                {
                    sitemapLinkOptions.UrlOptions.LanguageEmbedding = LanguageEmbedding.Always;
                    // generate x-default
                    var href = GetFullLink(item, sitemapLinkOptionsForXDefaultLink);
                    var hreflang = "x-default";
                    var alternate = BuildAlternateLinkElement(href, hreflang);
                    alternateUrls.Insert(0, alternate);
                }
                else
                {
                    sitemapLinkOptions.UrlOptions.LanguageEmbedding = LanguageEmbedding.Never;
                }

                sitemapLinkOptions.UrlOptions.Language = item.Language;
                var loc = GetFullLink(item, sitemapLinkOptions);
                var lastmod = GetUpdatedDate(item);
                var changefreq = changeFreq.ToString().ToLowerInvariant();
                var priority = GetPriority(item);
                var page = BuildPageElement(loc, lastmod, changefreq, priority, alternateUrls);
                pages.Add(page);

            }
            var document = BuildXmlDocument(pages);
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = new StringWriter(builder))
            {
                document.Save(writer);
            }
            FixDeclaration(builder);
            return builder;
        }
    }
}