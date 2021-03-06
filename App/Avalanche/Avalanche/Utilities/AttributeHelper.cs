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
using System.Text;
using Xamarin.Forms;

namespace Avalanche.Utilities
{
    public static class AttributeHelper
    {
        private static Dictionary<string, TypeConverter> typeConverters = new Dictionary<string, TypeConverter>
        {
            {typeof(Accelerator).Name, new AcceleratorTypeConverter() },
            {typeof(Binding).Name, new BindingTypeConverter() },
            {typeof(Rectangle).Name, new BoundsTypeConverter() },
            {typeof(Color).Name, new ColorTypeConverter()},
            {typeof(Constraint).Name, new  ConstraintTypeConverter()},
            {typeof(Font).Name, new FontTypeConverter() },
            {typeof(GridLength).Name, new GridLengthTypeConverter() },
            {typeof(Keyboard).Name, new KeyboardTypeConverter() },
            // Image source converter added as a comment to say this conversion is handled specially
            // because of the FFLoadingCache has a slightly different converter
            // {typeof(ImageSource).Name, new ImageSourceConverter() }, 
            {typeof(List<string>).Name, new ListStringTypeConverter() },
            {typeof(LayoutOptions).Name, new LayoutOptionsConverter() },
            {typeof(Point).Name, new PointTypeConverter() },
            {typeof(double).Name, new FontSizeConverter() },
            {typeof(Thickness).Name, new ThicknessTypeConverter() },
            {typeof(Type).Name, new TypeTypeConverter() },
            {typeof(Uri).Name, new Xamarin.Forms.UriTypeConverter() },
            {typeof(WebViewSource).Name, new  WebViewSourceTypeConverter()}
        };


        public static void ApplyTranslation( object obj, Dictionary<string, string> attributes )
        {
            foreach ( var attribute in attributes )
            {
                if ( string.IsNullOrWhiteSpace( attribute.Value ) )
                {
                    continue;
                }

                var property = obj.GetType().GetProperty( attribute.Key );
                if ( property == null || !property.CanWrite )
                {
                    continue;
                }

                if ( property.PropertyType == typeof( string ) )
                {
                    property.SetValue( obj, attribute.Value );
                }
                else if ( property.PropertyType == typeof( int ) )
                {
                    int.TryParse( attribute.Value, out int value );
                    property.SetValue( obj, value );
                }
                else if ( property.PropertyType == typeof( ImageSource ) )
                {
                    if ( obj is FFImageLoading.Forms.CachedImage )
                    {
                        property.SetValue( obj, new FFImageLoading.Forms.ImageSourceConverter().ConvertFromInvariantString( attribute.Value ) );
                    }
                    else
                    {
                        property.SetValue( obj, new ImageSourceConverter().ConvertFromInvariantString( attribute.Value ) );
                    }
                }
                else if ( typeConverters.ContainsKey( property.PropertyType.Name ) )
                {
                    property.SetValue( obj, typeConverters[property.PropertyType.Name].ConvertFromInvariantString( attribute.Value ) );
                }
            }
        }

        public static void HandleActionItem( Dictionary<string, string> Attributes )
        {
            if ( !Attributes.ContainsKey( "ActionType" ) || Attributes["ActionType"] == "0" )
            {
                return;
            }

            var resource = "";
            if ( Attributes.ContainsKey( "Resource" ) )
            {
                resource = Attributes["Resource"];
            }

            var parameter = "";
            if ( Attributes.ContainsKey( "Parameter" ) )
            {
                parameter = Attributes["Parameter"];
            }

            if ( Attributes["ActionType"] == "1" && !string.IsNullOrWhiteSpace( resource ) ) //push new page
            {
                AvalancheNavigation.GetPage( Attributes["Resource"], parameter );
            }
            else if ( Attributes["ActionType"] == "2" && !string.IsNullOrWhiteSpace( resource ) ) //replace
            {
                AvalancheNavigation.ReplacePage( Attributes["Resource"], parameter );
            }
            else if ( Attributes["ActionType"] == "3" ) //pop page
            {
                AvalancheNavigation.RemovePage();
            }
            else if ( Attributes["ActionType"] == "4" && !string.IsNullOrWhiteSpace( resource ) )
            {
                if ( !string.IsNullOrWhiteSpace( parameter ) )
                {
                    if ( resource.Contains( "?" ) )
                    {
                        resource += "&rckipid=" + parameter;
                    }
                    else
                    {
                        resource += "?rckipid=" + parameter;
                    }
                }
                Device.OpenUri( new Uri( resource ) );
            }
        }

    }
}
