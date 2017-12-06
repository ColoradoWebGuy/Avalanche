﻿using System;
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
                var mainGrid = this.FindByName<Grid>( "MainGrid" );
                if ( mainGrid != null )
                {
                    var image = new FFImageLoading.Forms.CachedImage()
                    {
                        Aspect = Aspect.AspectFill,
                        Source = value

                    };
                    mainGrid.Children.Add( image );
                }
            }
        }
    }
}