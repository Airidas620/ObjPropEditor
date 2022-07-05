using SciChart.Charting.Visuals;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace JSONConfFileEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            SciChartSurface.SetRuntimeLicenseKey("Qbd+bMWPoNiK8vP5p94Gc07O++HWH7Az4rvSMIIPNdp97Pj50CIXd47O5yvRc4oKeu8CPyVGuFFYYv08ifVqN3sNy6CziRYXZzei5GNwtShOinzkQymTGX+jT1PBjJEIrwzaB9sdYWYnEON5zjbIXZsrQnB/3Un21TMSxiZ5D0DXn3Fpx48f43BLfS+X0HXu5Dneq/wy2IivfuTMF1bUE4W5BC4pjtTHW9wGFXkEK8kR4sU3jFax4TnwwFfitvIzYDAGk/zu8IYoGNLmK0OFv2v9S+NzSEiuOvsOQdZTYM5nco4SFdIYEwkNJ9ahE458X2//706iOnFpCZvz4L2A0K0EwEX3pzmSh82p2J2t2AqD+D3d+6QhevCXxQhnBdnPkpVCS1SO4NWh/pP79gcYvoHTnzTEBDm6xT0w9BR4SJErjUAqmKBc1RLWKv6V0gEkNDGkI3gex+EQXJFaVZ3KQRxlDGQCIcNNd7EMxHp8zOM=");
        }
    }
}
