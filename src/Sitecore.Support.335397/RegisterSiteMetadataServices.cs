namespace Sitecore.Support.XA.Feature.SiteMetadata.Pipelines.IOC
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using System;
    using Sitecore;
    using Sitecore.XA.Feature.SiteMetadata.Repositories.BrowserTitle;
    using Sitecore.XA.Feature.SiteMetadata.Repositories.Favicon;
    using Sitecore.XA.Feature.SiteMetadata.Models;
    using Sitecore.XA.Feature.SiteMetadata.Sitemap;
    using Sitecore.XA.Foundation.Mvc.Repositories.Base;
    using Sitecore.XA.Feature.SiteMetadata.Repositories.SeoMetadata;
    using Sitecore.XA.Feature.SiteMetadata.Repositories.TwitterMetadata;
    using Sitecore.XA.Feature.SiteMetadata.Repositories.OpenGraphMetadata;
    using Sitecore.XA.Feature.SiteMetadata.Repositories.Viewport;

    public class RegisterSiteMetadataServices : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IBrowserTitleRepository, BrowserTitleRepository>();
            serviceCollection.AddSingleton<IFaviconRepository, FaviconRepository>();
            serviceCollection.AddSingleton<IAbstractRepository<SeoMetadataRenderingModel>, SeoMetadataRepository>();
            serviceCollection.AddSingleton<IAbstractRepository<TwitterMetadataRenderingModel>, TwitterMetadataRepository>();
            serviceCollection.AddSingleton<IAbstractRepository<OpenGraphRenderingModel>, OpenGraphMetadataRepository>();
            serviceCollection.AddTransient<ISitemapGenerator, Sitecore.Support.XA.Feature.SiteMetadata.Sitemap.SitemapGenerator>();
            serviceCollection.AddSingleton<IAbstractRepository<ViewportRenderingModel>, ViewportRepository>();
        }
    }
}