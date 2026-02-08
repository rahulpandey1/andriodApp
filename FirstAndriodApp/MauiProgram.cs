using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using FirstAndriodApp.Pages;
using FirstAndriodApp.PageModels;

namespace FirstAndriodApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
    				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
    				{
    					handler.PlatformView.SingleSelectionFollowsFocus = false;
    				});

    				Microsoft.Maui.Handlers.ContentViewHandler.Mapper.AppendToMapping(nameof(Pages.Controls.CategoryChart), (handler, view) =>
    				{
    					if (view is Pages.Controls.CategoryChart && handler.PlatformView is Microsoft.Maui.Platform.ContentPanel contentPanel)
    					{
    						contentPanel.IsTabStop = true;
    					}
    				});
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            builder.Services.AddSingleton<ProjectRepository>();
            builder.Services.AddSingleton<TaskRepository>();
            builder.Services.AddSingleton<CategoryRepository>();
            builder.Services.AddSingleton<TagRepository>();
            builder.Services.AddSingleton<SeedDataService>();
            builder.Services.AddSingleton<ModalErrorHandler>();
            builder.Services.AddSingleton<MainPageModel>();
            builder.Services.AddSingleton<ProjectListPageModel>();
            builder.Services.AddSingleton<ManageMetaPageModel>();
            builder.Services.AddSingleton<MatchItPageModel>();

            // Game Services
            builder.Services.AddSingleton<IAudioService, AudioService>();
            builder.Services.AddSingleton<IGameManager, GameManager>();
            builder.Services.AddSingleton<LevelService>();

            builder.Services.AddSingleton<GamesViewModel>();
            builder.Services.AddTransientWithShellRoute<GamesPage, GamesViewModel>("games");

            builder.Services.AddTransient<MemoryFlipPageModel>();
            builder.Services.AddTransientWithShellRoute<MemoryFlipPage, MemoryFlipPageModel>("memoryflip");

            builder.Services.AddTransient<ShadowMatchPageModel>();
            builder.Services.AddTransientWithShellRoute<ShadowMatchPage, ShadowMatchPageModel>("shadowmatch");

            builder.Services.AddTransient<ColorMatchPageModel>();
            builder.Services.AddTransientWithShellRoute<ColorMatchPage, ColorMatchPageModel>("colormatch");

            builder.Services.AddTransient<ShapeSorterPageModel>();
            builder.Services.AddTransientWithShellRoute<ShapeSorterPage, ShapeSorterPageModel>("shapesorter");

            builder.Services.AddTransient<AnimalMatchPageModel>();
            builder.Services.AddTransientWithShellRoute<AnimalMatchPage, AnimalMatchPageModel>("animalmatch");

            builder.Services.AddTransient<PuzzleSliderPageModel>();
            builder.Services.AddTransientWithShellRoute<PuzzleSliderPage, PuzzleSliderPageModel>("puzzleslider");

            builder.Services.AddTransient<JigsawPageModel>();
            builder.Services.AddTransientWithShellRoute<JigsawPage, JigsawPageModel>("jigsawpuzzle");

            builder.Services.AddTransient<TracingPageModel>();
            builder.Services.AddTransientWithShellRoute<TracingPage, TracingPageModel>("alphabettracing");

            builder.Services.AddTransient<ConnectDotsPageModel>();
            builder.Services.AddTransientWithShellRoute<ConnectDotsPage, ConnectDotsPageModel>("connectdots");

            builder.Services.AddTransient<SpotDifferencePageModel>();
            builder.Services.AddTransientWithShellRoute<SpotDifferencePage, SpotDifferencePageModel>("spotdifference");

            builder.Services.AddTransient<ColorMixingPageModel>();
            builder.Services.AddTransientWithShellRoute<ColorMixingPage, ColorMixingPageModel>("colormixing");

            builder.Services.AddTransient<FruitCatchPageModel>();
            builder.Services.AddTransientWithShellRoute<FruitCatchPage, FruitCatchPageModel>("fruitcatch");

            builder.Services.AddTransient<MazePageModel>();
            builder.Services.AddTransientWithShellRoute<MazePage, MazePageModel>("mazerunner");

            builder.Services.AddTransient<BalloonPopPageModel>();
            builder.Services.AddTransientWithShellRoute<BalloonPopPage, BalloonPopPageModel>("balloonpop");

            builder.Services.AddTransientWithShellRoute<ProjectDetailPage, ProjectDetailPageModel>("project");
            builder.Services.AddTransientWithShellRoute<TaskDetailPage, TaskDetailPageModel>("task");
            builder.Services.AddTransientWithShellRoute<MatchItPage, MatchItPageModel>("matchit");

            return builder.Build();
        }
    }
}
