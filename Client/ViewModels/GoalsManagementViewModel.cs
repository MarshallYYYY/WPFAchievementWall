using Client.Events;
using Client.Services;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class GoalsManagementViewModel : BindableBase
    {
        public GoalsManagementViewModel(GoalService goalService)
        {
            service = goalService;
            _ = LoadingHelper.RunWithLoadingAsync(InitGolas);
        }
        private readonly GoalService service;
        public ObservableCollection<Goal> Goals { get; set; } = [];
        private async Task InitGolas()
        {
            List<Goal> goals = await service.GetGoalsAsync();

            Goals.Clear();
            //foreach (Goal goal in goals)
            //    Goals.Add(goal);
            //goals.ForEach(goal => Goals.Add(goal));
            goals.ForEach(Goals.Add);
        }
    }
}
