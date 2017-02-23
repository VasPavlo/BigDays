
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using UniversalImageLoader.Core;
using UniversalImageLoader.Cache.Disc.Naming;
using UniversalImageLoader.Core.Assist;

namespace BigDays
{
    [Application]
    public class BigDaysApplication : Application
    {

        protected BigDaysApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
#if DEBUG
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread)
            {
                StrictMode.SetThreadPolicy(new StrictMode.ThreadPolicy.Builder().DetectAll().PenaltyLog().Build());
                StrictMode.SetVmPolicy(new StrictMode.VmPolicy.Builder().DetectAll().PenaltyLog().Build());
            }
#endif

            base.OnCreate();

            InitImageLoader(ApplicationContext);
        }

        public static void InitImageLoader(Context context)
        {
            // This configuration tuning is custom. 
            // You can tune every option, you may tune some of them, or you can create default configuration
            // with:
            //   ImageLoaderConfiguration.CreateDefault(this);
            var config = new ImageLoaderConfiguration.Builder(context);
            config.ThreadPriority(Java.Lang.Thread.NormPriority - 2);
            config.DenyCacheImageMultipleSizesInMemory();
            config.DiskCacheFileNameGenerator(new Md5FileNameGenerator());
            config.DiskCacheSize(50 * 1024 * 1024); // 50 MiB
            config.TasksProcessingOrder(QueueProcessingType.Lifo);
            config.WriteDebugLogs(); // Remove for release app

            // Initialize ImageLoader with configuration.
            ImageLoader.Instance.Init(config.Build());
        }
    }
}
