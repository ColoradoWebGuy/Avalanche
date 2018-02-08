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
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using Avalanche.Models;
using Rock;
using Rock.Attribute;
using Rock.Web.UI;

namespace Avalanche
{
    [KeyValueListField( "Custom Attributes", "Custom attributes to set on block.", false, keyPrompt: "Attribute", valuePrompt: "Value" )]
    public abstract class AvalancheBlockCustomSettings : RockBlockCustomSettings, IMobileResource
    {
        private Dictionary<string, string> _customAtributes;
        public Dictionary<string, string> CustomAttributes
        {
            get
            {
                if ( _customAtributes == null )
                {
                    _customAtributes = new Dictionary<string, string>();
                    var customs = GetAttributeValue( "CustomAttributes" ).ToKeyValuePairList();
                    foreach ( var item in customs )
                    {
                        _customAtributes[item.Key] = HttpUtility.UrlDecode( ( string ) item.Value );
                    }
                }
                return _customAtributes;
            }
        }

        public abstract MobileBlock GetMobile( string parameter );
        public virtual MobileBlockResponse HandleRequest( string request, Dictionary<string, string> Body )
        {
            return new MobileBlockResponse()
            {
                Request = request,
                Response = "",
                TTL = 0
            };
        }
        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );

            if ( CurrentUser != null && UserCanAdministrate )
            {
                var mobileBlock = this.GetMobile( "" );
                var atts = "";
                if ( mobileBlock.Attributes != null )
                {
                    atts = string.Join( "<br>", mobileBlock.Attributes.Select( x => x.Key + ": " + x.Value ) );
                }
                HtmlGenericControl div = new HtmlGenericControl( "div" );
                div.InnerHtml = string.Format( @"
<details style=""margin:0px 0px -18px -18px"">
    <summary><i class='fa fa-info-circle'></i></summary>
    <div class=""mobileBlockInformation"">
        <div class=""mobileBlockInformationHeader"">
            <b>{0}</b>
        </div>
        <div style=""padding:3px 20px"">
            {1}
        </div>
    </div>
</details>",
                mobileBlock.BlockType,
                atts
                );
                this.Controls.AddAt( 0, div );
            }
            else
            {
                this.Visible = false;
            }
        }
    }
}