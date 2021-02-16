﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;
using Autofac;
using GitTrends.Mobile.Common;
using GitTrends.Mobile.Common.Constants;
using GitTrends.Shared;
using Shiny;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using static GitTrends.MarkupExtensions;
using static Xamarin.Forms.Markup.GridRowsColumns;

namespace GitTrends
{
    public class RepositoryPage : BaseContentPage<RepositoryViewModel>, ISearchPage
    {
        readonly WeakEventManager<string> _searchTextChangedEventManager = new WeakEventManager<string>();

        readonly RefreshView _refreshView;
        readonly FirstRunService _firstRunService;
        readonly GitHubUserService _gitHubUserService;
        readonly DeepLinkingService _deepLinkingService;

        public RepositoryPage(IMainThread mainThread,
                                FirstRunService firstRunService,
                                IAnalyticsService analyticsService,
                                GitHubUserService gitHubUserService,
                                DeepLinkingService deepLinkingService,
                                RepositoryViewModel repositoryViewModel,
                                MobileSortingService mobileSortingService) : base(repositoryViewModel, analyticsService, mainThread)
        {
            _firstRunService = firstRunService;
            _gitHubUserService = gitHubUserService;
            _deepLinkingService = deepLinkingService;

            SearchBarTextChanged += HandleSearchBarTextChanged;
            RepositoryViewModel.PullToRefreshFailed += HandlePullToRefreshFailed;
            LanguageService.PreferredLanguageChanged += HandlePreferredLanguageChanged;

            this.SetBinding(TitleProperty, nameof(RepositoryViewModel.TitleText));

            ToolbarItems.Add(new ToolbarItem
            {
                Text = PageTitles.SettingsPage,
                IconImageSource = Device.RuntimePlatform is Device.iOS ? "Settings" : null,
                Order = Device.RuntimePlatform is Device.Android ? ToolbarItemOrder.Secondary : ToolbarItemOrder.Default,
                AutomationId = RepositoryPageAutomationIds.SettingsButton,
                Command = new AsyncCommand(ExecuteSetttingsToolbarItemCommand)
            });

            ToolbarItems.Add(new ToolbarItem
            {
                Text = RepositoryPageConstants.SortToolbarItemText,
                Priority = 1,
                IconImageSource = Device.RuntimePlatform is Device.iOS ? "Sort" : null,
                Order = Device.RuntimePlatform is Device.Android ? ToolbarItemOrder.Secondary : ToolbarItemOrder.Default,
                AutomationId = RepositoryPageAutomationIds.SortButton,
                Command = new AsyncCommand(ExecuteSortToolbarItemCommand)
            });

            Content = new Grid
            {
                RowDefinitions = Rows.Define(
                    (Row.Totals, AbsoluteGridLength(125)),
                    (Row.CollectionView, Star),
                    (Row.Information, AbsoluteGridLength(100))),

                ColumnDefinitions = Columns.Define(
                    (Column.CollectionView, Star),
                    (Column.Information, AbsoluteGridLength(100))),

                Children =
                {
                    new InformationLabel().Row(Row.Totals).ColumnSpan(All<Column>())
                        .Bind<Label, bool, bool>(IsVisibleProperty,nameof(RepositoryViewModel.IsRefreshing), convert: isRefreshing => !isRefreshing)
                        .Bind<Label, IReadOnlyList<Repository>, string>(Label.TextProperty, nameof(RepositoryViewModel.VisibleRepositoryList), convert: repositories => StatisticsService.GetInformationLabelText(repositories, mobileSortingService)),

                    new RefreshView
                    {
                        AutomationId = RepositoryPageAutomationIds.RefreshView,
                        Content = new CollectionView
                        {
                            ItemTemplate = new RepositoryDataTemplateSelector(mobileSortingService),
                            BackgroundColor = Color.Transparent,
                            SelectionMode = SelectionMode.Single,
                            AutomationId = RepositoryPageAutomationIds.CollectionView,
                            //Work around for https://github.com/xamarin/Xamarin.Forms/issues/9879
                            Header = Device.RuntimePlatform is Device.Android ? new BoxView { HeightRequest = BaseRepositoryDataTemplate.BottomPadding } : null,
                            Footer = Device.RuntimePlatform is Device.Android ? new BoxView { HeightRequest = BaseRepositoryDataTemplate.TopPadding } : null,
                            EmptyView = new EmptyDataView("EmptyRepositoriesList", RepositoryPageAutomationIds.EmptyDataView)
                                            .Bind<EmptyDataView, bool, bool>(IsVisibleProperty, nameof(RepositoryViewModel.IsRefreshing), convert: isRefreshing => !isRefreshing)
                                            .Bind(EmptyDataView.TitleProperty, nameof(RepositoryViewModel.EmptyDataViewTitle))
                                            .Bind(EmptyDataView.DescriptionProperty, nameof(RepositoryViewModel.EmptyDataViewDescription))

                        }.Bind(CollectionView.ItemsSourceProperty, nameof(RepositoryViewModel.VisibleRepositoryList))
                         .Invoke(collectionView => collectionView.SelectionChanged += HandleCollectionViewSelectionChanged)

                    }.RowSpan(All<Row>()).ColumnSpan(All<Column>()).Assign(out _refreshView)
                     .Bind(RefreshView.IsRefreshingProperty, nameof(RepositoryViewModel.IsRefreshing))
                     .Bind(RefreshView.CommandProperty, nameof(RepositoryViewModel.PullToRefreshCommand))
                     .DynamicResource(RefreshView.RefreshColorProperty, nameof(BaseTheme.PullToRefreshColor)),
                }
            }.Assign(out Grid grid);

            if (Device.RuntimePlatform is Device.Android)
            {
                grid.Children.Add(new InformationButton(mobileSortingService, mainThread, analyticsService).Row(Row.Information).Column(Column.Information));
            }
        }

        enum Row { Totals, CollectionView, Information }
        enum Column { CollectionView, Information }

        public event EventHandler<string> SearchBarTextChanged
        {
            add => _searchTextChangedEventManager.AddEventHandler(value);
            remove => _searchTextChangedEventManager.RemoveEventHandler(value);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var token = await _gitHubUserService.GetGitHubToken();

            if (!_firstRunService.IsFirstRun && shouldShowWelcomePage(Navigation, token.AccessToken))
            {
                var welcomePage = ContainerService.Container.Resolve<WelcomePage>();

                //Allow RepositoryPage to appear briefly before loading 
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                await Navigation.PushModalAsync(welcomePage);
            }
            else if (!_firstRunService.IsFirstRun
                        && isUserValid(token.AccessToken, _gitHubUserService.Alias)
                        && _refreshView.Content is CollectionView collectionView
                        && collectionView.ItemsSource.IsNullOrEmpty())
            {
                _refreshView.IsRefreshing = true;
            }

            bool shouldShowWelcomePage(in INavigation navigation, in string accessToken)
            {
                return !navigation.ModalStack.Any()
                        && _gitHubUserService.Alias != DemoUserConstants.Alias
                        && !isUserValid(accessToken, _gitHubUserService.Alias);
            }

            static bool isUserValid(in string accessToken, in string alias) => !string.IsNullOrWhiteSpace(accessToken) || !string.IsNullOrWhiteSpace(alias);
        }

        async void HandleCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var collectionView = (CollectionView)sender;
            collectionView.SelectedItem = null;

            if (e?.CurrentSelection.FirstOrDefault() is Repository repository)
            {
                AnalyticsService.Track("Repository Tapped", new Dictionary<string, string>
                {
                    { nameof(Repository) + nameof(Repository.OwnerLogin), repository.OwnerLogin },
                    { nameof(Repository) + nameof(Repository.Name), repository.Name }
                });

                await NavigateToTrendsPage(repository);
            }
        }

        Task NavigateToSettingsPage()
        {
            var settingsPage = ContainerService.Container.Resolve<SettingsPage>();
            return MainThread.InvokeOnMainThreadAsync(() => Navigation.PushAsync(settingsPage));
        }

        Task NavigateToTrendsPage(Repository repository)
        {
            var trendsPage = ContainerService.Container.Resolve<TrendsPage>(new TypedParameter(typeof(Repository), repository));
            return MainThread.InvokeOnMainThreadAsync(() => Navigation.PushAsync(trendsPage));
        }

        Task ExecuteSetttingsToolbarItemCommand()
        {
            AnalyticsService.Track("Settings Button Tapped");

            return NavigateToSettingsPage();
        }

        async Task ExecuteSortToolbarItemCommand()
        {
            var sortingOptions = MobileSortingService.SortingOptionsDictionary.Values;

            string? selection = await DisplayActionSheet(SortingConstants.ActionSheetTitle, SortingConstants.CancelText, null, sortingOptions.ToArray());

            if (!string.IsNullOrWhiteSpace(selection) && selection != SortingConstants.CancelText)
                ViewModel.SortRepositoriesCommand.Execute(MobileSortingService.SortingOptionsDictionary.First(x => x.Value == selection).Key);
        }

        void HandlePullToRefreshFailed(object sender, PullToRefreshFailedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (!Application.Current.MainPage.Navigation.ModalStack.Any()
                    && Application.Current.MainPage.Navigation.NavigationStack.Last() is RepositoryPage)
                {
                    if (e.Accept is null)
                    {
                        await DisplayAlert(e.Title, e.Message, e.Cancel);
                    }
                    else
                    {
                        var isAccepted = await DisplayAlert(e.Title, e.Message, e.Accept, e.Cancel);
                        if (isAccepted)
                            await _deepLinkingService.OpenBrowser(GitHubConstants.GitHubRateLimitingDocs);
                    }
                }
            });
        }

        void HandlePreferredLanguageChanged(object sender, string? e)
        {
            var sortItem = ToolbarItems.First(x => x.AutomationId is RepositoryPageAutomationIds.SortButton);
            var settingsItem = ToolbarItems.First(x => x.AutomationId is RepositoryPageAutomationIds.SettingsButton);

            sortItem.Text = RepositoryPageConstants.SortToolbarItemText;
            settingsItem.Text = PageTitles.SettingsPage;
        }

        void HandleSearchBarTextChanged(object sender, string searchBarText) => ViewModel.FilterRepositoriesCommand.Execute(searchBarText);

        void ISearchPage.OnSearchBarTextChanged(in string text) => _searchTextChangedEventManager.RaiseEvent(this, text, nameof(SearchBarTextChanged));

        class InformationLabel : StatisticsLabel
        {
            public InformationLabel() : base(string.Empty, true, nameof(BaseTheme.PrimaryTextColor))
            {
                AutomationId = RepositoryPageAutomationIds.InformationLabel;
                this.FillExpand().TextCenter();
            }
        }
    }
}
