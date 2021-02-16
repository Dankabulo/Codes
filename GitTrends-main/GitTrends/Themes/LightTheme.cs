﻿using Sharpnado.MaterialFrame;
using Xamarin.Forms;

namespace GitTrends
{
    public class LightTheme : BaseTheme
    {
        const string _primaryTextHex = "#584053";
        const string _primaryTealHex = LightTealColorHex;
        const string _primaryDarkTealHex = "#1C473D";
        const string _accentYellowHex = "#FCBC66";
        const string _accentOrangeHex = CoralColorHex;
        const string _toolbarTextHex = "#FFFFFF";
        const string _textHex = "#121212";
        const string _iconColor = "#121212";
        const string _buttonTextColor = "#FFFFFF";
        const string _accentLightBlueHex = "#66A7FC";
        const string _accentPurpleHex = "#8F3795";
        const string _pageBackgroundColorHex = "#F1F1F1";
        const string _cardSurfaceHex = "#FFFFFF";
        const string _toolbarSurfaceHex = LightTealColorHex;
        const string _circleImageBackgroundHex = "#FFFFFF";
        const string _githubButtonSurfaceHex = "#231F20";

        //Graph Palette
        const string _saturatedLightBlueHex = "#2BC3BE";
        const string _saturatedIndigoHex = "#5D6AB1";
        const string _saturatedYellowHex = "#FFC534";
        const string _saturatedPinkHex = "#F2726F";

        //Blended Colors
        const string _black12PercentBlend = "#E0E0E0";

        //Surfaces
        public override Color NavigationBarBackgroundColor { get; } = Color.FromHex(_toolbarSurfaceHex);
        public override Color NavigationBarTextColor { get; } = Color.FromHex(_toolbarTextHex);

        public override Color PageBackgroundColor { get; } = Color.FromHex(_pageBackgroundColorHex);
        public override Color PageBackgroundColor_85Opactity { get; } = Color.FromHex(_pageBackgroundColorHex).MultiplyAlpha(0.85);

        //Text
        public override Color PrimaryTextColor { get; } = Color.FromHex(_primaryTextHex);
        public override Color TextColor { get; } = Color.FromHex(_textHex);


        //Chart
        public override Color TotalViewsColor { get; } = Color.FromHex(_saturatedLightBlueHex);
        public override Color TotalUniqueViewsColor { get; } = Color.FromHex(_saturatedIndigoHex);
        public override Color TotalClonesColor { get; } = Color.FromHex(_saturatedYellowHex);
        public override Color TotalUniqueClonesColor { get; } = Color.FromHex(_saturatedPinkHex);

        public override Color ChartAxisTextColor { get; } = Color.FromHex(_primaryTextHex);
        public override Color ChartAxisLineColor { get; } = Color.FromHex(_primaryTextHex);

        //Components
        //Splash
        public override Color SplashScreenStatusColor { get; } = Color.FromHex(_primaryTextHex);

        //Icons
        public override Color IconColor { get; } = Color.FromHex(_iconColor);
        public override Color IconPrimaryColor { get; } = Color.FromHex(_primaryTealHex);

        //Buttons
        public override Color ButtonTextColor { get; } = Color.FromHex(_buttonTextColor);
        public override Color ButtonBackgroundColor { get; } = Color.FromHex(_primaryTealHex);

        //Indicators
        public override Color ActivityIndicatorColor { get; } = Color.FromHex(_primaryDarkTealHex);
        public override Color PullToRefreshColor { get; } = Color.FromHex(_toolbarSurfaceHex);

        //Card
        public override Color CardSurfaceColor { get; } = Color.FromHex(_cardSurfaceHex);
        public override Color CardBorderColor { get; } = Color.FromHex(_black12PercentBlend);

        public override Color SeparatorColor { get; } = Color.FromHex(_black12PercentBlend);

        //Card Stats Color
        public override Color CardStarsStatsTextColor { get; } = Color.FromHex(_textHex);
        public override Color CardStarsStatsIconColor { get; } = Color.FromHex(_accentYellowHex);
        public override Color CardForksStatsTextColor { get; } = Color.FromHex(_textHex);
        public override Color CardForksStatsIconColor { get; } = Color.FromHex(_primaryTealHex);
        public override Color CardIssuesStatsTextColor { get; } = Color.FromHex(_textHex);
        public override Color CardIssuesStatsIconColor { get; } = Color.FromHex(_accentOrangeHex);
        public override Color CardViewsStatsTextColor { get; } = Color.FromHex(_textHex);
        public override Color CardViewsStatsIconColor { get; } = Color.FromHex(_saturatedLightBlueHex);
        public override Color CardClonesStatsTextColor { get; } = Color.FromHex(_textHex);
        public override Color CardClonesStatsIconColor { get; } = Color.FromHex(_saturatedYellowHex);
        public override Color CardUniqueViewsStatsTextColor { get; } = Color.FromHex(_textHex);
        public override Color CardUniqueViewsStatsIconColor { get; } = Color.FromHex(_saturatedIndigoHex);
        public override Color CardUniqueClonesStatsTextColor { get; } = Color.FromHex(_textHex);
        public override Color CardUniqueClonesStatsIconColor { get; } = Color.FromHex(_saturatedPinkHex);
        public override Color CardTrendingStatsColor { get; } = Color.FromHex(_primaryTealHex);

        //Settings Components
        public override Color SettingsLabelTextColor { get; } = Color.FromHex(_primaryTextHex);
        public override Color BorderButtonBorderColor { get; } = Color.FromHex(_black12PercentBlend);
        public override Color BorderButtonFontColor { get; } = Color.FromHex(_primaryTextHex);
        public override Color TrendsChartSettingsSelectionIndicatorColor { get; } = Color.FromHex(_primaryTealHex);

        public override Color GitTrendsImageBackgroundColor { get; } = Color.White;

        public override Color GitHubButtonSurfaceColor { get; } = Color.FromHex(_githubButtonSurfaceHex);

        public override Color GitHubHandleColor { get; } = Color.FromHex(_textHex);

        public override Color PrimaryColor { get; } = Color.FromHex(_primaryTealHex);

        public override Color CloseButtonTextColor { get; } = Color.FromHex(_toolbarTextHex);
        public override Color CloseButtonBackgroundColor { get; } = Color.FromHex(_toolbarSurfaceHex);

        public override Color PickerBorderColor { get; } = Color.LightGray;

        public override string GitTrendsImageSource { get; } = "GitTrendsGreen";
        public override string DefaultProfileImageSource { get; } = "DefaultProfileImageGreen";
        public override string DefaultReferringSiteImageSource { get; } = "DefaultReferringSiteImage";

        public override MaterialFrame.Theme MaterialFrameTheme { get; } = MaterialFrame.Theme.Light;
    }
}
