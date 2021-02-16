﻿using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Xamarin.UITest;

namespace GitTrends.UITests
{
    [TestFixture(Platform.Android, UserType.LoggedIn)]
    [TestFixture(Platform.iOS, UserType.LoggedIn)]
    class AppStoreScreenShots : BaseTest
    {
        public AppStoreScreenShots(Platform platform, UserType userType) : base(platform, userType)
        {
        }

        [Test]
        public async Task AppStoreScreenShotsTest()
        {
            //Arrange
            var screenRect = App.Query().First().Rect;

            //Act
            App.Screenshot("Repository Page Light");

            RepositoryPage.TapRepository(RepositoryPage.VisibleCollection.First().Name);

            await TrendsPage.WaitForPageToLoad().ConfigureAwait(false);

            App.TouchAndHoldCoordinates(screenRect.CenterX, screenRect.CenterY);
            App.Screenshot("Trends Page Light");

            TrendsPage.TapReferringSitesButton();
            await ReferringSitesPage.WaitForPageToLoad().ConfigureAwait(false);

            App.Screenshot("Referring Sites Page Light");

            ReferringSitesPage.ClosePage();
            await TrendsPage.WaitForPageToLoad().ConfigureAwait(false);

            TrendsPage.TapBackButton();
            await RepositoryPage.WaitForPageToLoad().ConfigureAwait(false);

            RepositoryPage.TapSettingsButton();
            await SettingsPage.WaitForPageToLoad().ConfigureAwait(false);

            App.Screenshot("Settings Page Light");

            SettingsPage.SelectTheme(Mobile.Shared.PreferredTheme.Dark);
            App.Screenshot("Settings Page Dark");

            SettingsPage.TapBackButton();
            await RepositoryPage.WaitForPageToLoad().ConfigureAwait(false);

            App.Screenshot("Repository Page Dark");

            RepositoryPage.TapRepository(RepositoryPage.VisibleCollection.Skip(2).First().Name);

            await TrendsPage.WaitForPageToLoad().ConfigureAwait(false);

            App.TouchAndHoldCoordinates(screenRect.CenterX, screenRect.CenterY);
            App.Screenshot("Trends Page Dark");

            TrendsPage.TapReferringSitesButton();
            await ReferringSitesPage.WaitForPageToLoad().ConfigureAwait(false);

            App.Screenshot("Referring Sites Page Dark");

            //Assert
        }
    }
}
