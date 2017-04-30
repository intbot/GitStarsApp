using System;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Views;
using FFImageLoading.Transformations;

namespace GitStarsApp.Droid
{
    class OnclickListener : Java.Lang.Object, View.IOnClickListener
    {
        public void OnClick(View v)
        {
            HandleOnClick();
        }
        public Action HandleOnClick { get; set; }
    }


    class RepoAdapter : BaseAdapter<Repo>
    {

        Activity _context;
        public List<Repo> Repos { get; set; }

        public RepoAdapter(Activity context, List<Repo> repos)
        {
            this._context = context;
            this.Repos = repos;
        }

        public override int Count => Repos.Count;

        public override Repo this[int position] => Repos[position];

        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            RepoAdapterViewHolder holder = null;
            var repo = this[position];

            if (view != null)
                holder = view.Tag as RepoAdapterViewHolder;

            if (holder == null)
            {
                holder = new RepoAdapterViewHolder();
                
                view = _context.LayoutInflater.Inflate(Resource.Layout.RepoItem, null);
                
                holder.NameTextView = view.FindViewById<TextView>(Resource.Id.nameTextView);
                holder.DescrTextView = view.FindViewById<TextView>(Resource.Id.descrTextView);
                holder.OwnerTextView = view.FindViewById<TextView>(Resource.Id.ownerTextView);
                holder.AvatarImageView = view.FindViewById<ImageViewAsync>(Resource.Id.avatarImageView);
                holder.RepoItemLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.repoItemLinearLayout);
                view.Tag = holder;
            }


            //fill in your items
            var onRepoItemClickListner = new OnclickListener();
            onRepoItemClickListner.HandleOnClick = async () =>
            {
                // TODO: This is not working because of a bug in share plugin. The bug is raised by me here:
                // Though James said it is fixed with 6.0 beta, it seems to be not.
                // https://github.com/jguertl/SharePlugin/issues/41
                await Util.OpenBrowser(repo.Url);
            };
            holder.RepoItemLinearLayout.SetOnClickListener(onRepoItemClickListner);

            holder.NameTextView.Text = repo.Name;
            holder.DescrTextView.Text = repo.Description;
            holder.OwnerTextView.Text = $"Language: {repo.Language} Built By: {repo.Owner} - Stars: {repo.Stars}";
            ImageService.Instance.LoadUrl(repo.AvatarUrl)
                //.LoadingPlaceholder("Icon.png")
                .FadeAnimation(true)
                //.Transform(new CornersTransformation(10, CornerTransformType.AllCut))
                .Retry(3, 200)
                .DownSample(100, 100)
                .Into(holder.AvatarImageView);

            return view;
        }
    }

    class RepoAdapterViewHolder : Java.Lang.Object
    {
        public LinearLayout RepoItemLinearLayout { get; set; }
        public TextView NameTextView { get; set; }
        public TextView DescrTextView { get; set; }
        public TextView OwnerTextView { get; set; }
        public ImageViewAsync AvatarImageView { get; set; }
    }
}