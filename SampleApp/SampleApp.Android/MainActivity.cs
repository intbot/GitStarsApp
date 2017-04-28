using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;

namespace GitStarsApp.Droid
{
	[Activity (Label = "Git Stars", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

        private GitHubService service = new GitHubService();

		protected async override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            var repos = await service.GetReposAsync();

            var repoListView = this.FindViewById<ListView>(Resource.Id.reoListView);
            repoListView.Adapter = new RepoAdapter(this, repos);

            // Get our button from the layout resource,
            // and attach an event to it
            var btnWeek = FindViewById<Button>(Resource.Id.btnWeek);
            var btnMonth = FindViewById<Button>(Resource.Id.btnMonth);
            var btnYear = FindViewById<Button>(Resource.Id.btnYear);

            btnWeek.Click += async (sender, e) => await UpdateList(sender, e, 7);
            btnMonth.Click += async (sender, e) => await UpdateList(sender, e, 30);
            btnYear.Click += async (sender, e) => await UpdateList(sender, e, 365);
        }

        private async Task UpdateList(object sender, System.EventArgs e, int days)
        {
            var repos = await service.GetReposAsync(days);
            var repoListView = this.FindViewById<ListView>(Resource.Id.reoListView);
            var adapter = repoListView.Adapter as RepoAdapter;
            adapter.Repos = repos;
            adapter.NotifyDataSetChanged();
        }
    }
}


