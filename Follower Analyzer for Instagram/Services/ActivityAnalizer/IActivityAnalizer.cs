using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Follower_Analyzer_for_Instagram.Services.ActivityAnalizer
{
    interface IActivityAnalizer
    {
        void StartAnalizing(CancellationTokenSource cancellationTokenSource);
    }
}
