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
using Avalanche.Models;
using Xamarin.Forms;

namespace Avalanche
{
    public class AvalanchePage : ContentPage
    {
        private string _backgroundImage;

        public new string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                _backgroundImage = value;
                AddBackgroundImage();
            }
        }

        public void AddBackgroundImage()
        {
            var mainGrid = this.FindByName<Grid>( "MainGrid" );
            if ( mainGrid != null )
            {
                var image = new FFImageLoading.Forms.CachedImage()
                {
                    Aspect = Aspect.AspectFill,
                    Source = _backgroundImage

                };
                mainGrid.Children.Add( image );
            }
        }

    }
}