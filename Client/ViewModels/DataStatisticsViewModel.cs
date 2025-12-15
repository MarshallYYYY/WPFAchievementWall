using Client.Common;
using Client.Models;
using Client.Services;
using Client.Services.WebApi;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Models;
using Newtonsoft.Json.Linq;
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
                    ILoadingService loadingService,
                    ISnackbarService snackbarService)
        {
            this.userSession = userSession;
            this.achievementService = achievementService;
            this.goalService = goalService;
            this.loadingService = loadingService;
            this.snackbarService = snackbarService;

            PlotCommand = new DelegateCommand(Plot);

            /*
            // 设置第一个ComboBox的选项为成就
            SelectedAnalysisObject = AnalysisObject.Achievement;
            // 设置第二个ComboBox的选项为年份
            SelectedDataDimension = DataDimension.Year;
            // 设置第三个ComboBox的选项为折线图
            SelectedChartType = ChartType.LineChart;

            // 设置右侧TabControl显示的图表类型为折线图
            TabControlSelectedIndex = 0;
            */
            SetDataDimensionOptions();
            SetChartTypeOptions();
        }

        #region 会话、服务
        private readonly IUserSession userSession;
        private readonly IAchievementService achievementService;
        private readonly IGoalService goalService;
        private readonly ILoadingService loadingService;
        private readonly ISnackbarService snackbarService;
        #endregion

        #region INavigationAware
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Init();
        }
        #endregion

        #region 每次进入页面时的数据初始化和之前选择的图表刷新
        private void Init()
        {
            // 加载数据，然后显示图表
            _ = loadingService.RunWithLoadingAsync(async () =>
            {
                await InitData();
                Plot();
            });
        }
        private List<Achievement> achievements = [];
        private readonly List<YearAchievements> groupedByYearAchievements = [];
        private readonly ObservableCollection<Goal> ongoingGoals = [];
        private readonly ObservableCollection<Goal> achievedGoals = [];
        private async Task InitData()
        {
            ApiResult<List<Achievement>> apiResult = await achievementService.GetUserAchievementsdAsync(
                userSession.CurrentUser.Id);
            if (apiResult.IsSuccess is false)
            {
                snackbarService.SendMessage(apiResult.ErrorMessage!);
            }
            else
            {
                achievements = apiResult.Data!;
            }

            ApiResult achievementApiResult = await achievementService.SetUserAchievementsGroupedByYearAsync(
                userSession.CurrentUser.Id, groupedByYearAchievements);
            if (achievementApiResult.IsSuccess is false)
            {
                snackbarService.SendMessage(achievementApiResult.ErrorMessage!);
            }

            ApiResult goalApiResult = await goalService.SplitUserGoalsAsync(
                userSession.CurrentUser.Id, ongoingGoals, achievedGoals);
            if (goalApiResult.IsSuccess is false)
            {
                snackbarService.SendMessage(goalApiResult.ErrorMessage!);
            }
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
                    DataDimensionOptions.Add(DataDimension.OntimeAchieved);
                    break;
            }
            SelectedDataDimension = DataDimensionOptions.First();
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

        #region 图表类型
        /// <summary>
        /// 第三个ComboBox（图表类型）的Source
        /// </summary>
        public ObservableCollection<string> ChartTypeOptions { get; } = [];
        private string selectedChartType = ChartType.LineChart;
        public string SelectedChartType
        {
            get { return selectedChartType; }
            set { SetProperty(ref selectedChartType, value); }
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
            switch (selectedChartType)
            {
                case ChartType.LineChart:
                    PlotYearLineChart();
                    break;
                case ChartType.BarChart:
                    PlotBarChart();
                    break;
                case ChartType.PieChart:
                    PlotPieChart();
                    break;
            }
        }

        #region 折线图 LineChart
        private ISeries[] lineSeries = [];
        public ISeries[] LineSeries
        {
            get { return lineSeries; }
            set { SetProperty(ref lineSeries, value); }
        }
        public Axis[] LineXAxes { get; } =
          // 365.25 是考虑了闰年的情况
          [new DateTimeAxis(TimeSpan.FromDays(365.25), date => date.ToString("yyyy")),];

        private void PlotYearLineChart()
        {
            // 点击绘制按钮后，改变右侧TabControl显示的图表类型。
            TabControlSelectedIndex = 0;

            //ObservableCollection<DateTimePoint> points = [];
            List<DateTimePoint> points = [];
            foreach (YearAchievements yearAchievements in groupedByYearAchievements)
            {
                // Convert.ToDateTime(yearAchievements.Year) 会失败，所以在后面补充 1月1日。
                DateTime dateTime = new(yearAchievements.Year ?? 2000, 1, 1);
                DateTimePoint dateTimePoint = new(dateTime, yearAchievements.Achievements.Count);
                points.Add(dateTimePoint);
            }
            LineSeries<DateTimePoint> lineSeries = new()
            {
                Values = points,

                //Fill = null, // 不要填充
                //Stroke = new SolidColorPaint(SKColors.SteelBlue) // 固定颜色

                // 让数值显示在数据点的上方
                DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                DataLabelsPosition = DataLabelsPosition.Top
            };
            LineSeries = [lineSeries,];
        }
        #endregion

        #region 柱状图 BarChart
        private ISeries[] barSeries = [];
        public ISeries[] BarSeries
        {
            get { return barSeries; }
            set { SetProperty(ref barSeries, value); }
        }
        private Axis[] barXAxes = [];
        public Axis[] BarXAxes
        {
            get { return barXAxes; }
            set { SetProperty(ref barXAxes, value); }
        }
        private void PlotBarChart()
        {
            TabControlSelectedIndex = 1;
            switch (selectedDataDimension)
            {
                case DataDimension.Year:
                    PlotYearBarChart();
                    //PlotYearBarChartPlanB();
                    break;
                case DataDimension.Level:
                    PlotBarChartTemplate(SetLevelChartData);
                    break;
                case DataDimension.Category:
                    PlotBarChartTemplate(SetCategoryChartData);
                    break;

                case DataDimension.CompletionStatus:
                    PlotBarChartTemplate(SetCompletionStatusChartData);
                    break;
                case DataDimension.OntimeAchieved:
                    PlotBarChartTemplate(SetOntimeAchievedChartData);
                    break;
            }
        }

        #region 绘制年份柱状图、其余四个数据维度通用的设置Series和Axis的函数
        private void PlotYearBarChart()
        {
            List<DateTimePoint> points = [];
            foreach (YearAchievements yearAchievements in groupedByYearAchievements)
            {
                DateTime dateTime = new(yearAchievements.Year ?? 2000, 1, 1);
                DateTimePoint dateTimePoint = new(dateTime, yearAchievements.Achievements.Count);
                points.Add(dateTimePoint);
            }
            ColumnSeries<DateTimePoint> columnSeries = new()
            {
                Values = points,

                //Fill = new SolidColorPaint(SKColors.LightSkyBlue),
                DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                DataLabelsPosition = DataLabelsPosition.Top
            };
            BarSeries = [columnSeries,];

            BarXAxes = [new DateTimeAxis(TimeSpan.FromDays(365.25), date => date.ToString("yyyy")),];
        }
        [Obsolete("这种绘制方式体现不出时间的跨度，比如2000与2005会紧靠着，不会有间隔距离上的体现。")]
        private void PlotYearBarChartPlanB()
        {
            PlotBarChartTemplate((values, labels) =>
            {
                foreach (YearAchievements yearAchievements in groupedByYearAchievements)
                {
                    values.Add(yearAchievements.Achievements.Count);
                    labels.Add(yearAchievements.Year.ToString()!);
                }
            });
        }

        /// <summary>
        /// 绘制柱状图的模板函数：设置柱状图的Series和Axis
        /// </summary>
        /// <param name="action">对values和labels添加元素</param>
        private void PlotBarChartTemplate(Action<List<int>, List<string>> action)
        {
            List<int> values = [];
            List<string> labels = [];

            action?.Invoke(values, labels);

            ColumnSeries<int> columnSeries = new()
            {
                Values = values,
                DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
                DataLabelsPosition = DataLabelsPosition.Top
            };
            Axis axis = new()
            {
                Labels = labels,

                LabelsRotation = 0,
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true,
                // By default the axis tries to optimize the number of 
                // names to fit the available space, 
                // when you need to force the axis to show all the names then you must: 
                ForceStepToMin = true,
                MinStep = 1
            };

            BarSeries = [columnSeries,];
            BarXAxes = [axis,];
        }
        #endregion
        #endregion

        #region 饼图 PieChart
        private IEnumerable<ISeries> pieSeries = [];
        public IEnumerable<ISeries> PieSeries
        {
            get { return pieSeries; }
            set { SetProperty(ref pieSeries, value); }
        }
        private void PlotPieChart()
        {
            TabControlSelectedIndex = 2;
            switch (selectedDataDimension)
            {
                case DataDimension.Year:
                    PlotPieChartTemplate(SetYearPieChartData);
                    break;
                case DataDimension.Level:
                    PlotPieChartTemplate(SetLevelChartData);
                    break;
                case DataDimension.Category:
                    PlotPieChartTemplate(SetCategoryChartData);
                    break;

                case DataDimension.CompletionStatus:
                    PlotPieChartTemplate(SetCompletionStatusChartData);
                    break;
                case DataDimension.OntimeAchieved:
                    PlotPieChartTemplate(SetOntimeAchievedChartData);
                    break;
            }
        }

        #region 设置年份饼图的数据（values和names）、通用的设置Series的函数
        private void SetYearPieChartData(List<int> values, List<string> names)
        {
            foreach (YearAchievements yearAchievements in groupedByYearAchievements)
            {
                values.Add(yearAchievements.Achievements.Count);
                names.Add(yearAchievements.Year.ToString()!);
            }
        }
        private void PlotPieChartTemplate(Action<List<int>, List<string>> action)
        {
            List<int> values = [];
            List<string> names = [];

            action?.Invoke(values, names);

            int index = 0;
            PieSeries = values.AsPieSeries((value, series) =>
            {
                series.Name = names[index++];

                series.DataLabelsPosition = PolarLabelsPosition.Middle;
                series.DataLabelsSize = 18;
                series.DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30));
                series.DataLabelsFormatter = point =>
                   $"{series.Name}: {point.Coordinate.PrimaryValue}/{point.StackedValue!.Total}";
                series.ToolTipLabelFormatter = point => $"{point.StackedValue!.Share:P2}";
            });
        }
        #endregion
        #endregion

        #region 设置 星级、类别、完成情况、准时达成 柱状图和饼图的数据（values和labels/names）
        private void SetLevelChartData(List<int> values, List<string> names)
        {
            for (int i = 1; i <= 5; i++)
            {
                values.Add(achievements.Where(a => a.Level == i).Count());
                names.Add($"星级 {i}");
            }
        }
        private void SetCategoryChartData(List<int> values, List<string> names)
        {
            names.AddRange([
                AchievementCategory.Default,
                AchievementCategory.Life,
                AchievementCategory.Learning,
                AchievementCategory.Health,
                AchievementCategory.Career,
            ]);
            for (int i = 0; i < names.Count; i++)
            {
                values.Add(achievements.Where(a => a.Category == names[i]).Count());
            }
        }
        private void SetCompletionStatusChartData(List<int> values, List<string> names)
        {
            names.AddRange(["进行中", "已完成"]);
            values.Add(ongoingGoals.Count);
            values.Add(achievedGoals.Count);
        }
        private void SetOntimeAchievedChartData(List<int> values, List<string> names)
        {
            names.AddRange(["未准时达成", "准时达成"]);
            // 只比较年月日，不比较时分秒
            values.Add(achievedGoals.Where(
                a => (a.AchieveDate ?? DateTime.Now).Date > a.TargetDate.Date).Count());
            values.Add(achievedGoals.Where(
                a => (a.AchieveDate ?? DateTime.Now).Date <= a.TargetDate.Date).Count());
        }
        #endregion 设置 星级、类别、完成情况、准时达成 柱状图和饼图的数据（values和labels/names）

        #endregion 右侧的TabControl
    }
}