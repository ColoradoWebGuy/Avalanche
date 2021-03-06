﻿// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using Avalanche.Models;
using Newtonsoft.Json;
using Rock;
using Rock.Model;
using Rock.Rest;
using Rock.Rest.Filters;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace Avalanche.Rest.Controllers
{
    public class AvalancheController : ApiControllerBase
    {
        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/home" )]
        public MobilePage GetHome()
        {
            return GetPage( GlobalAttributesCache.Value( "AvalancheHomePage" ).AsInteger() );
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/page/{id}" )]
        [System.Web.Http.Route( "api/avalanche/page/{id}/{*parameter}" )]
        public MobilePage GetPage( int id, string parameter = "" )
        {
            var person = GetPerson();
            HttpContext.Current.Items.Add( "CurrentPerson", person );

            var pageCache = PageCache.Read( id );
            if ( !pageCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                return new MobilePage();
            }
            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            MobilePage mobilePage = new MobilePage();
            mobilePage.LayoutType = pageCache.Layout.Name;
            mobilePage.Title = pageCache.PageTitle;
            mobilePage.ShowTitle = pageCache.PageDisplayTitle;
            foreach ( var attribute in pageCache.AttributeValues )
            {
                mobilePage.Attributes.Add( attribute.Key, attribute.Value.ValueFormatted );
            }
            foreach ( var block in pageCache.Blocks )
            {
                if ( block.IsAuthorized( Authorization.VIEW, person ) )
                {
                    var blockCache = BlockCache.Read( block.Id );
                    var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                    if ( control is RockBlock && control is IMobileResource )
                    {
                        control.SetBlock( pageCache, blockCache );
                        var mobileResource = control as IMobileResource;
                        var mobileBlock = mobileResource.GetMobile( parameter );
                        mobileBlock.BlockId = blockCache.Id;
                        mobileBlock.Zone = blockCache.Zone;
                        mobilePage.Blocks.Add( mobileBlock );
                    }
                }
            }
            HttpContext.Current.Response.Headers.Set( "TTL", pageCache.OutputCacheDuration.ToString() );
            return mobilePage;
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/block/{id}" )]
        [System.Web.Http.Route( "api/avalanche/block/{id}/{*request}" )]
        public MobileBlockResponse BlockGetRequest( int id, string request = "" )
        {
            var person = GetPerson();
            HttpContext.Current.Items.Add( "CurrentPerson", person );
            var blockCache = BlockCache.Read( id );
            var pageCache = PageCache.Read( blockCache.PageId ?? 0 );
            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            if ( blockCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                if ( control is RockBlock && control is IMobileResource )
                {
                    control.SetBlock( pageCache, blockCache );
                    var mobileResource = control as IMobileResource;
                    var mobileBlockResponse = mobileResource.HandleRequest( request, new Dictionary<string, string>() );
                    HttpContext.Current.Response.Headers.Set( "TTL", mobileBlockResponse.TTL.ToString() );
                    return mobileBlockResponse;
                }
            }
            HttpContext.Current.Response.Headers.Set( "TTL", "0" );
            return new MobileBlockResponse();
        }


        [HttpPost]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/block/{id}" )]
        [System.Web.Http.Route( "api/avalanche/block/{id}/{*arg}" )]
        public MobileBlockResponse BlockPostRequest( int id, string arg = "" )
        {
            HttpContent requestContent = Request.Content;
            string content = requestContent.ReadAsStringAsync().Result;
            var body = JsonConvert.DeserializeObject<Dictionary<string, string>>( content );
            var person = GetPerson();
            HttpContext.Current.Items.Add( "CurrentPerson", person );
            var blockCache = BlockCache.Read( id );
            var pageCache = PageCache.Read( blockCache.PageId ?? 0 );
            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            if ( blockCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                if ( control is RockBlock && control is IMobileResource )
                {
                    control.SetBlock( pageCache, blockCache );
                    var mobileResource = control as IMobileResource;
                    var mobileBlockResponse = mobileResource.HandleRequest( arg, body );
                    mobileBlockResponse.TTL = 0;
                    return mobileBlockResponse;
                }
            }
            return new MobileBlockResponse();

        }
    }
}