using Client.Common;
using Client.Services;
using Client.Services.WebApi;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Client.ViewModels
{
    public class DataStatisticsViewModel : BindableBase, INavigationAware
    {
        public DataStatisticsViewModel(
                    IUserSession userSession,
                    IAchievementService achievementService,
                    IGoalService goalService,
                    ILoadingService loadingService)
        {
            this.userSession = userSession;
            this.achievementService = achievementService;
            this.goalService = goalService;
            this.loadingService = loadingService;

            PlotCommand = new DelegateCommand(Plot);
        }

        #region 会话、服务
        private readonly IUserSession userSession;
        private readonly IAchievementService achievementService;
        private readonly IGoalService goalService;
        private readonly ILoadingService loadingService;
        #endregion

        #region INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Init();
        }
        #endregion

        #region 每次进入页面时的初始化
        private void Init()
        {
            SelectedAnalysisObject = AnalysisObject.Achievement;
            SelectedDataDimension = DataDimension.Year;
            SetDataDimensionOptions();
            SetChartTypeOptions();
        }
        private void SetDataDimensionOptions()
        {
            DataDimensionOptions.Clear();
            switch (selectedAnalysisObject)
            {
                case AnalysisObject.Achievement:
                    DataDimensionOptions.Add(DataDimension.Year);
                    DataDimensionOptions.Add(DataDimension.Level);
                    DataDimensionOptions.Add(DataDimension.Category);
                    break;
                case AnalysisObject.Goal:
                    DataDimensionOptions.Add(DataDimension.CompletionStatus);
                    DataDimensionOptions.Add(DataDimension.OntimeAchievementRate);
                    break;
            }
            SelectedDataDimension = DataDimensionOptions.First();
        }
        private void SetChartTypeOptions()
        {
            ChartTypeOptions.Clear();
            if (selectedDataDimension == DataDimension.Year)
            {
                ChartTypeOptions.Add(ChartType.LineChart);
            }
            ChartTypeOptions.Add(ChartType.BarChart);
            ChartTypeOptions.Add(ChartType.PieChart);
            SelectedChartType = ChartTypeOptions.First();
        }
        #endregion

        #region 左侧的三个ComboBox
        #region 分析对象
        // 第一个ComboBox的Source一直不变
        public ObservableCollection<string> AnalysisObjectOptions { get; } =
            [AnalysisObject.Achievement, AnalysisObject.Goal];
        private string selectedAnalysisObject = AnalysisObject.Achievement;
        public string SelectedAnalysisObject
        {
            get { return selectedAnalysisObject; }
            set
            {
                SetProperty(ref selectedAnalysisObject, value);
                // 当第一个ComboBox的选项改变时，设置第二个ComboBox的Source。
                SetDataDimensionOptions();
            }
        }
        #endregion

        #region 数据维度
        /// <summary>
        /// 第二个ComboBox（数据维度）的Source 
        /// </summary>
        public ObservableCollection<string> DataDimensionOptions { get; } = [];
        private string selectedDataDimension = DataDimension.Year;
        public string SelectedDataDimension
        {
            get { return selectedDataDimension; }
            set
            {
                SetProperty(ref selectedDataDimension, value);
                // 当第二个ComboBox的选项改变时，设置第三个ComboBox的Source。
                SetChartTypeOptions();
            }
        }
        #endregion

        #region 图表类型
        /// <summary>
        /// 第三个ComboBox（图表类型）的Source
        /// </summary>
        public ObservableCollection<string> ChartTypeOptions { get; } = [];
        private string selectedChartType = ChartType.LineChart;
        public string SelectedChartType
        {
            get { return selectedChartType; }
            set
            {
                SetProperty(ref selectedChartType, value);
                // 当第三个ComboBox的选项改变时，改变右侧TabControl显示的图表。
                switch (value)
                {
                    case ChartType.LineChart:
                        TabControlSelectedIndex = 0;
                        break;
                    case ChartType.BarChart:
                        TabControlSelectedIndex = 1;
                        break;
                    case ChartType.PieChart:
                        TabControlSelectedIndex = 2;
                        break;
                }
            }
        }
        #endregion 
        #endregion

        #region 右侧的TabControl
        private int tabControlSelectedIndex = 0;
        public int TabControlSelectedIndex
        {
            get { return tabControlSelectedIndex; }
            set { SetProperty(ref tabControlSelectedIndex, value); }
        }
        public DelegateCommand PlotCommand { get; private set; }
        private void Plot()
        {
            TabControlSelectedIndex = tabControlSelectedIndex == 0 ? 1 : 0;
        }
        public ISeries[] LineSeries { get; set; } =
        [
        new LineSeries<DateTimePoint>
        {
            Values = new ObservableCollection<DateTimePoint>
            {
                new DateTimePoint(new DateTime(2005, 1, 1), 3),
                new DateTimePoint(new DateTime(2006, 1, 2), 6),
                new DateTimePoint(new DateTime(2008, 1, 3), 5),
                new DateTimePoint(new DateTime(2010, 1, 4), 3),
                new DateTimePoint(new DateTime(2015, 1, 5), 5),
                new DateTimePoint(new DateTime(2020, 1, 6), 8),
                new DateTimePoint(new DateTime(2025, 1, 7), 6)
            }
        },
    ];
        public Axis[] LineXAxes { get; } =
          // 365.25 是考虑了闰年的情况
          [new DateTimeAxis(TimeSpan.FromDays(365.25), date => date.ToString("yyyy")),];
        #endregion
    }
}