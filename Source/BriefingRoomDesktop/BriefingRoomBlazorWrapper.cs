using System.Drawing;
using System.Windows.Forms;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;

namespace BriefingRoomDesktop
{
    public partial class BriefingRoomBlazorWrapper : Form
    {
        public BriefingRoomBlazorWrapper()
        {
            InitializeComponent();
            Icon = new Icon("Media\\Icon.ico");
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddBlazorWebView();
            serviceCollection.AddBlazoredLocalStorage();
            var blazor = new BlazorWebView()
            {
                Dock = DockStyle.Fill,
                HostPage = "wwwroot/index.html",
                Services = serviceCollection.BuildServiceProvider(),
            };
            blazor.RootComponents.Add<App>("#app");
            Controls.Add(blazor);
        }

    }
}
