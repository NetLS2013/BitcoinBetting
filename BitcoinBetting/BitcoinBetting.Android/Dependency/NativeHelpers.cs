using BitcoinBetting.Core.Interfaces;
using BitcoinBetting.Droid.Dependency;

[assembly: Xamarin.Forms.Dependency(typeof(NativeHelpers))]
namespace BitcoinBetting.Droid.Dependency
{
    public class NativeHelpers : INativeHelpers
    {
        public void CloseApp()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}